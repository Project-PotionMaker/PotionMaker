using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace Kamgam.SettingsGenerator
{
    public partial class CreateSettingUGUIWindow : EditorWindow
    {
        public StyleSheet m_StyleSheet;

        const string StyleSheedName = "SettingsMenuCreatorWindow";
        public StyleSheet GetStyleSheet()
        {
            if (m_StyleSheet == null)
            {
                var guids = AssetDatabase.FindAssets("t:StyleSheet " + StyleSheedName);
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                }
            }

            return m_StyleSheet;
        }

        [System.NonSerialized]
        public static CreateSettingUGUIWindow LastOpenedWindow;

        [MenuItem("Window/Settings Creator", priority = 0)]
        [MenuItem("Tools/Settings Generator/Setting Creator Window", priority = 0)]
        [MenuItem("GameObject/UI/Settings Generator/Setting Creator Window", priority = 2001)]
        public static void ShowMenuCreator()
        {
            LastOpenedWindow = GetWindow<CreateSettingUGUIWindow>();
        }

        [MenuItem("GameObject/UI/Settings Generator/Create Settings Initializer", priority = 2002)]
        public static void CreateSettingsInitializer()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab SettingsInitializer");
            var guid = guids.First(g => AssetDatabase.GUIDToAssetPath(g).EndsWith("/SettingsInitializer.prefab"));

            if (string.IsNullOrEmpty(guid))
                return;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab == null)
                return;

            var instance = PrefabUtility.InstantiatePrefab(prefab, null) as GameObject;
            EditorUtility.SetDirty(instance);
            Undo.RegisterCompleteObjectUndo(instance, "Settings Initializer Created");
        }

        [MenuItem("GameObject/UI/Settings Generator/Create Settings Applier", priority = 2002)]
        public static void CreateSettingsApplier()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab SettingsApplier");
            var guid = guids.First(g => AssetDatabase.GUIDToAssetPath(g).EndsWith("/SettingsApplier.prefab"));

            if (string.IsNullOrEmpty(guid))
                return;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab == null)
                return;

            var instance = PrefabUtility.InstantiatePrefab(prefab, null) as GameObject;
            EditorUtility.SetDirty(instance);
            Undo.RegisterCompleteObjectUndo(instance, "Settings Applier Created");
        }

        public static void CenterOnMainWin()
        {
            Rect main = EditorGUIUtility.GetMainWindowPosition();
            Rect pos = LastOpenedWindow.position;
            float centerWidth = (main.width - pos.width) * 0.5f;
            float centerHeight = (main.height - pos.height) * 0.5f;
            pos.x = main.x + centerWidth;
            pos.y = main.y + centerHeight;
            LastOpenedWindow.position = pos;
        }

        protected VisualElement _tabbar;
        protected VisualElement _providerCtn;
        protected VisualElement _visualsCtn;

        private SerializedObject _serializedWindow;

        public void CreateGUI()
        {
            LastOpenedWindow = this;
            
            // Delay needed due to Unity bug:
            // https://issuetracker.unity3d.com/issues/editorwindow-fails-to-open-when-the-cause-of-nullreferenceexception-in-creategui-is-fixed
            EditorApplication.delayCall += () =>
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode && !docked)
                {
                    //var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/Images/gear.png"); 
                    LastOpenedWindow.titleContent = new GUIContent("Setting Creator"); //, icon);
                    LastOpenedWindow.minSize = new Vector2(200, 200);

                    // If too small then resize.
                    var pos = LastOpenedWindow.position;
                    pos.width = Mathf.Max(600, pos.width);
                    pos.height = Mathf.Max(600, pos.height);
                    LastOpenedWindow.position = pos;

                    // Center window only once per session.
                    bool centered = SessionState.GetBool("Kamgam.SettingsGenerator.Window.Centered", false);
                    if (!centered)
                        CenterOnMainWin();
                    SessionState.SetBool("Kamgam.SettingsGenerator.Window.Centered", true);
                }
            };

            var root = rootVisualElement;
            root.styleSheets.Add(GetStyleSheet());
            root.Clear();
            if (EditorGUIUtility.isProSkin)
                root.AddToClassList("dark");    
            else
                root.AddToClassList("light");    

            // Data
            _serializedWindow = new SerializedObject(this);
            _serializedWindow.Update();
            
            Config.SerializedObject.Update();

            // Make sure rootVisualElement has the focus when we start.
            rootVisualElement.focusable = true;
            rootVisualElement.pickingMode = PickingMode.Position;
            rootVisualElement.Focus();

            Selection.selectionChanged -= onSelectionChanged;
            Selection.selectionChanged += onSelectionChanged;

            EditorApplication.hierarchyChanged -= onHierarchyChanged;
            EditorApplication.hierarchyChanged += onHierarchyChanged;
            
            // header bar
            var tabContainers = new List<VisualElement>();
            _tabbar = rootVisualElement.AddTabbar(new List<string>() { "Configuration", "User Interface (UI)" }, tabContainers, activeTab: 0, onTabButtonPressed:
                (b, i, l) =>
                {
                    using var e = ClickEvent.GetPooled();
                    if (i == 0) onShowChooseProvider(e);
                    if (i == 1) onShowChooseVisual(e);
                });
            _tabbar.AddToClassList("dont-shrink");
            _tabbar.name = "MainTabBar";
            var helpBtn = _tabbar.AddButton("?", (e) => Application.OpenURL(Installer.ManualUrl));
            helpBtn.tooltip = "Opens the manual in your browser.";
            
            var providerCtn = createChooseProviderGUI(root);
            var visualsCtn = createChooseVisualGUI(root);
            var addSettingCtn = createAddSettingUI(root);
            
            // Add create UIs to tabbar
            tabContainers.Add(providerCtn);
            tabContainers.Add(visualsCtn);
            
            onShowChooseProvider(null);
        }

        private void getState(
            out bool hasCustomProvider,
            out bool hasDefaultProvider,
            out bool isDemoScene,
            out List<SettingsProvider> providersInScene,
            out bool noProviderInSceneButDefaultExists,
            out bool conflictBetweenDefaultAndScene,
            out bool isNewUser
        )
        {
            hasCustomProvider = SettingsProvider.DoesCustomProviderExist();
            
            var config = SettingsGeneratorSettings.GetOrCreate();
            hasDefaultProvider = config.HasDefaultProvider;

            // Case: Example scene opened (skip tho settings selection)
            isDemoScene = this.isDemoScene();

            providersInScene = SettingsProvider.EditorFindAllProvidersInLoadedScenes(excludeExampleProviders: !isDemoScene);
                
            // Case: Provider in scene but empty default (and it's NOT an example scene) -> update default.
            if (!config.HasDefaultProvider && providersInScene.Count > 0 && !isDemoScene)
            {
                config.DefaultProvider = providersInScene[0];
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
                Logger.LogMessage($"Using settings '{config.DefaultProvider.name}' as default because we found it in the current scene.");
            }
            
            // Case: Default exists but none in scene -> Skip provider selection.
            noProviderInSceneButDefaultExists = providersInScene.Count == 0 && hasDefaultProvider;

            // Case: Conflict between scene and default (ask to update scene or default).
            conflictBetweenDefaultAndScene = hasDefaultProvider && providersInScene.Count > 0 && config.DefaultProvider != providersInScene[0];

            // Case: None in scene and no default (completely new user).
            isNewUser = !hasCustomProvider && !hasDefaultProvider && providersInScene.Count == 0;

        }

        private bool isDemoScene()
        {
            var resolver = GameObjectUtils.FindObjectOfType<SettingResolver>(includeInactive: true);
            if (resolver != null)
            {
                var path = resolver.gameObject.scene.path;
                return path.Contains("Kamgam");
            }

            return false;
        }

        public void OnDestroy()
        {
            LastOpenedWindow = null;
            Selection.selectionChanged -= onSelectionChanged;
            EditorApplication.hierarchyChanged -= onHierarchyChanged;
        }
    }
}