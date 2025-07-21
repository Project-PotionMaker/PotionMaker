using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

namespace Kamgam.SettingsGenerator
{
    // Manage existing settings in selected UI container view.
    
    public partial class CreateSettingUGUIWindow : EditorWindow
    {
        [SerializeField]
        protected GameObject _uiContainer;
        
        protected bool _uiContainerLocked = false;
        protected ListView _resolverListView;
        protected SettingResolver[] _resolvers = new SettingResolver[]{};
        protected Button _uiContainerLockedBtn;
        protected List<StyleChoice> _styleChoices = new List<StyleChoice>();

        public class StyleChoice
        {
            public string PrefabAssetPath;
            public string Name;
            public SettingData.DataType[] SupportedDataTypes;
            public System.Action<GameObject> ConvertFunc;
            
            protected GameObject _prefab;

            public StyleChoice(string prefabAssetPath, string name, SettingData.DataType[] supportedDataTypes, System.Action<GameObject> convertFunc)
            {
                PrefabAssetPath = prefabAssetPath;
                Name = StringUtils.InsertSpaceBeforeUpperCase(name);
                SupportedDataTypes = supportedDataTypes;
                ConvertFunc = convertFunc;
            }
            
            public GameObject GetPrefab()
            {
                if (_prefab == null)
                    _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);

                return _prefab;
            }
        }
        
        void updateStyleChoices()
        {
            _styleChoices.Clear();

            var guids = AssetDatabase.FindAssets("t:Prefab (Setting)");
            foreach (var guid in guids)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                string name = System.IO.Path.GetFileNameWithoutExtension(prefabPath);
                name = name.Replace(" (Setting)", "");

                GameObject go = PrefabUtility.LoadPrefabContents(prefabPath);

                var resolver = go.GetComponentInChildren<ISettingResolver>(includeInactive: true);
                if (resolver == null)
                {
                    PrefabUtility.UnloadPrefabContents(go);
                    continue;
                }

                // Find convert function in SettingsGeneratorPrefabStyleConverter
                System.Action<GameObject> convertFunc = SettingsGeneratorPrefabStyleConverter.GetConversionMethod(prefabPath);
                
                var entry = new StyleChoice(prefabPath, name, resolver.GetSupportedDataTypes(), convertFunc);
                _styleChoices.Add(entry);
                
                PrefabUtility.UnloadPrefabContents(go);
            }
        }

        private VisualElement createChooseVisualGUI(VisualElement root)
        {
            // Create gui
            _visualsCtn = root.AddContainer("ChooseVisual", "grow", "content-container");
            var ctn = _visualsCtn;

            ctn.AddLabel("User Interface (UI)", "h1");
            var providerObjField = ctn.AddObjectField<SettingsProvider>("Default Provider:", Config.DefaultProvider, onProviderChanged, allowSceneObjects: false, "dont-shrink");
            providerObjField.bindingPath = SettingsGeneratorSettings._DefaultProviderFieldName;
            providerObjField.Bind(Config.SerializedObject);
            providerObjField.SetEnabled(false);

            var uiContainerCtn = ctn.AddContainer("UIContainerCtn", "horizontal", "mb-10", "dont-shrink");
            var uiContainerField = uiContainerCtn.AddObjectField<GameObject>("UI Container:", null, null, allowSceneObjects: true, "grow");
            uiContainerField.bindingPath = "_uiContainer";
            uiContainerField.Bind(_serializedWindow);
            uiContainerField.SetEnabled(false);
            uiContainerField.tooltip = "New settings will be added to this container. Update by selecting a UI element in your scene.";
            _uiContainerLockedBtn = uiContainerCtn.AddButton("", onUIContainerLockClicked, "ui-container-lock", "shrink");
            _uiContainerLockedBtn.tooltip = "If enabled then the selected container will remain the same even if the selection is changed.";

            var listHeader = ctn.AddContainer("Header", "horizontal", "dont-shrink");
            listHeader.AddLabel("Setting (ID)", "ui-entry-header-name");
            listHeader.AddLabel("Style", "ui-entry-header-style");
            listHeader.AddLabel("Type", "ui-entry-header-type");
            listHeader.AddLabel("Actions", "ui-entry-header-actions");

            _resolverListView = new ListView();
            _resolverListView.AddToClassList("ui-resolver-list");
            _resolverListView.makeItem = makeItem;
            _resolverListView.selectionType = SelectionType.Multiple;
            _resolverListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _resolverListView.bindItem = (e, i) => bindItem(e, i, _resolverListView);
            _resolverListView.reorderable = true;
#if UNITY_2022_1_OR_NEWER
            _resolverListView.selectionChanged += (e) =>
#else
            _resolverListView.onSelectionChange += (e) =>
#endif
            {
                //
            };
#if UNITY_2021_2_OR_NEWER
            _resolverListView.itemIndexChanged += onResolverEntryIndexChanged;
#endif
            ctn.Add(_resolverListView);
            updateResolverList();
            updateStyleChoices();
            
            ctn.AddButton("+ Add New Setting", onShowAddSettingDialog, "ui-entry-create", "pt-10", "pb-10");
            
            ctn.RegisterCallback<KeyUpEvent>(onResolversKeyUp, TrickleDown.TrickleDown);

            return _visualsCtn;
        }
        
        private void onResolversKeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                var resolverIds = _resolverListView.selectedIndices;
                foreach (var index in resolverIds)
                {
                    var resolver = _resolvers[index];
                    Undo.DestroyObjectImmediate(resolver.gameObject);
                }

                updateResolverList();
            }
        }

        private void onShowChooseVisual(ClickEvent evt)
        {
            _serializedWindow.Update();
            Config.SerializedObject.Update();
            if (Config.DefaultProvider != null)
                Config.DefaultProvider.SerializedObject.Update();
            
            getState(
                out var hasCustomProvider,
                out var hasDefaultProvider,
                out var isDemoScene,
                out var providersInScene,
                out var noProviderInSceneButDefaultExists,
                out var conflictBetweenDefaultAndScene,
                out var isNewUser);
            
            _providerCtn.style.display = DisplayStyle.None;
            _visualsCtn.style.display = DisplayStyle.Flex;
            _addSettingContainer.style.display = DisplayStyle.None;

            if (_uiContainerLocked)
                _uiContainerLockedBtn.AddToClassList("locked");
            else
                _uiContainerLockedBtn.RemoveFromClassList("locked");
            
            updateSelectedUIContainer(Selection.activeGameObject);
            
            activateAllSettingsInUI();
            updateResolverList();
            updateStyleChoices();
            
            if (!isDemoScene)
            {
                SettingsProvider.EditorSetProviderInScene(Config.DefaultProvider, _uiContainer);
            }
        }
        
        private void onResolverEntryIndexChanged(int oldIndex, int newIndex)
        {
            _ignoreHierarchyChange = true;
            try
            {
                // Notice: Old and new are already swapped in _resolvers.
                int offset = (newIndex - oldIndex) < 0 ? -1 : 1; // find the previous one.
                var neighbour = _resolvers[newIndex - offset].gameObject.transform;
                int newSiblingIndex = neighbour.GetSiblingIndex();
                // Set parent too
                _resolvers[newIndex].gameObject.transform.SetParent(neighbour.parent, worldPositionStays: true);
                _resolvers[newIndex].gameObject.transform.SetSiblingIndex(newSiblingIndex);

                foreach (var resolver in _resolvers)
                {
                    updateNavigationOfUGUIInstance(resolver.gameObject);
                }
            }
            finally
            {
                // Delay because onHierarchyChanged is fired asynchronously so we have to wait for it to fire
                // (and be ignored) before we reset the flag.
                EditorApplication.delayCall += () =>
                {
                    _ignoreHierarchyChange = false;
                };
            }
        }

        void updateResolverList()
        {
            _resolvers = getResolversInContainer();
            _resolverListView.itemsSource = _resolvers;
            _resolverListView.Rebuild();
        }
        
        SettingResolver[] getResolversInContainer()
        {
            if (_uiContainer == null)
                return new SettingResolver[]{};

            if (_uiContainer == null)
                return new SettingResolver[]{};

            return _uiContainer.GetComponentsInChildren<SettingResolver>(includeInactive: true);
        }

        // Activates all settings in the scene. Useful to ensure a settings that was deactivated before is not re-activated.
        private void activateAllSettingsInUI()
        {
            int sceneCount = EditorSceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                    continue;

                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    var resolvers = root.GetComponentsInChildren<ISettingResolver>(includeInactive: true);
                    foreach (var resolver in resolvers)
                    {
                        var behaviour = resolver as MonoBehaviour;
                        var provider = resolver.GetProvider();
                        if (provider != null && provider.SettingsAsset != null)
                        {
                            if (provider.SettingsAsset.HasID(resolver.GetID()))
                            {
                                bool isActive = provider.SettingsAsset.GetActiveSetting(resolver.GetID()) != null;
                                if (!isActive)
                                {
                                    bool activate = EditorUtility.DisplayDialog("Found inactive setting!",
                                        $"Found inactive setting '{resolver.GetID()}'.\n\n" +
                                        $"Do you want to activate it now?",
                                        "Yes (recommended)", "No");
                                    if (activate)
                                    {
                                        provider.SettingsAsset.SetActive(resolver.GetID(), true);
                                        if (behaviour != null)
                                            EditorUtility.SetDirty(behaviour);
                                        Logger.LogMessage($"Activated setting '{resolver.GetID()}'.", behaviour);
                                    }
                                }
                            }
                            else
                            {
                                Logger.LogWarning($"Found setting UI with missing ID '{resolver.GetID()}' (click to go to object).", behaviour);
                            }
                        }
                    }
                }
            }
        }

        void onUIContainerLockClicked(ClickEvent evt)
        {
            (evt.target as VisualElement).ToggleInClassList("locked");
            _uiContainerLocked = !_uiContainerLocked;
        }
        
        void onSelectionChanged()
        {
            var previousContainer = _uiContainer;
            updateSelectedUIContainer(Selection.activeGameObject);

            if (_uiContainer != previousContainer)
            {
                updateResolverList();
            }
        }

        protected bool _ignoreHierarchyChange;

        void onHierarchyChanged()
        {
            if (_ignoreHierarchyChange)
                return;
            
            updateResolverList();
        }

        void updateSelectedUIContainer(GameObject go)
        {
            if (!_uiContainerLocked
                && go != null
                && !AssetDatabase.Contains(go)) // Object in scene or prefab stage
            {
                bool isValid = false;
                
                // TODO: Allow UIDocument once we support UI Toolkit setting creation.
                // UIToolkit: Check if it is a child of a canvas or a UI Document
                // if (go.TryGetComponent<UIDocument>(out _))
                //    isValid = true;

                // uGUI: Check if it is a child of a canvas.
                var canvas = go.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    isValid = true;
                    
                    // Check if selection is inside an object that has a resolver.
                    // If yes the change go to point to the parent.
                    var resolver = go.GetComponentInParent<ISettingResolver>(includeInactive: false);
                    if (resolver != null)
                    {
                        var resolverBehaviour = resolver as MonoBehaviour;
                        if (resolverBehaviour != null)
                        {
                            if (go == resolverBehaviour.gameObject ||
                                go.transform.IsChildOf(resolverBehaviour.transform))
                            {
                                if (resolverBehaviour.transform.parent != null)
                                {
                                    go = resolverBehaviour.transform.parent.gameObject;
                                    isValid = true;
                                }
                            }
                        }
                    }
                }

                if (isValid)
                {
                    _uiContainer = go;
                    EditorGUIUtility.PingObject(_uiContainer);
                }
            }
            else if (_uiContainer == null && go == null)
            {
                // If no ui container was selected then try and find one (prefer uGUI/Resolvers over UIDocument).
                var resolver = CompatibilityUtils.FindObjectOfType<SettingResolver>(includeInactive: false);
                if (resolver != null && resolver.transform.parent != null)
                {
                    _uiContainer = resolver.transform.parent.gameObject;
                    EditorGUIUtility.PingObject(_uiContainer);
                }
                else
                {
                    // Try canvas
                    var canvas = CompatibilityUtils.FindObjectOfType<Canvas>(includeInactive: true);
                    if (canvas != null)
                    {
                        _uiContainer = canvas.gameObject;
                        EditorGUIUtility.PingObject(_uiContainer);
                    }
                    // TODO: Enable once we support settings creation for UIDocument.
                    // else
                    // {
                    //     // Try UIDocument
                    //     var doc = GameObject.FindObjectOfType<UIDocument>(includeInactive: true);
                    //     if (doc != null)
                    //     {
                    //         _uiContainer = doc.gameObject;
                    //     }
                    // }
                }
            }
        }

        void onAddNewSetting(ClickEvent evt)
        {
            throw new System.NotImplementedException();
        }

        VisualElement makeItem()
        {
            var ctn = new VisualElement();
            ctn.AddToClassList("ui-entry");
            
            var rowCtn = ctn.AddContainer("Row", "ui-entry-row");

            var dragIcon = rowCtn.AddContainer("DragIcon");
            dragIcon.AddToClassList("ui-entry-drag-icon");
            dragIcon.tooltip = "Drag here to change the order of settings.";
            
            var nameLbl = rowCtn.AddLabel("Name", "ui-entry-name");
            nameLbl.tooltip = "Shows the ID of the setting.\n" +
                              "HINT: You can drag here to change the order of settings.";
            
            var styleDropDown = rowCtn.AddDropDown("Style", new List<string>() {}, null, "ui-entry-style" );
            styleDropDown.tooltip =
                "Changes the look of the setting in the UI. You can pick from multiple prefabs or templates.\n\n" +
                "HINT: If you append the string '(Setting)' to your prefabs then they will be listed here too (they need to have a ISettingResolver component).";

            var typeLbl = rowCtn.AddLabel("Int", "ui-entry-type");
            
            var editBtn = rowCtn.AddButton("Edit", null, "ui-entry-edit");
            editBtn.tooltip = "Opens the settings details for editing.";
            
            var resolverBtn = rowCtn.AddButton("UI", null, "ui-entry-resolver");
            resolverBtn.tooltip = "Selects the settings resolver on the ui.";

            var detailsCtn = ctn.AddContainer("UIEntryDetails", "ui-entry-details", "hidden");
            // The details ctn is filled on demand, see 

            return ctn;
        }

        private void makeAndBindItemDetails(VisualElement detailsCtn, int index, SettingResolver resolver)
        {
            if (resolver.SettingsProvider == null || resolver.SettingsProvider.SettingsAsset == null)
                return;

            var serializedProperty = resolver.SettingsProvider.SettingsAsset.GetSettingAsSerializedProperty(resolver.GetID());
            detailsCtn.AddPropertyField(serializedProperty);

            var deleteBtn = detailsCtn.AddButton("", onDeleteEntryClicked, "ui-entry-delete");
            deleteBtn.text = "Delete";
            deleteBtn.tooltip = "Removes this setting from the UI.\n\n" +
                                "NOTICE: The setting itself will not be deactivated in the settings list (see 'Configuration' tab). If you want the setting to be gone completely you will have to disable it in the configs.";
            deleteBtn.userData = resolver;
        }

        private void bindItem(VisualElement element, int index, ListView listView)
        {
            var resolver = listView.itemsSource[index] as SettingResolver;
            if (resolver == null)
                return;

            element.style.opacity = resolver.gameObject.activeInHierarchy ? 1f :0.5f;
            
            var nameLbl = element.Q<Label>(className:"ui-entry-name");
            nameLbl.text = resolver.GetID();
            
            var typeLbl = element.Q<Label>(className:"ui-entry-type");
            typeLbl.text = resolver.GetSupportedDataTypes()[0].ToString();
            
            // Styles
            // The we try to extract from the existing UI which style is currently being used and auto-select that.
            string currentStyle = "Custom";
            string currentStylePrefabAssetPath = null;
            if (PrefabUtility.IsPartOfPrefabInstance(resolver.gameObject))
            {
                var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(resolver.gameObject);
                
                // Get all the style paths
                var stylePrefabPaths = _styleChoices.Select(c => c.PrefabAssetPath);
                
                // Get all the paths of all the prefab ancestors.
                var parents = PrefabUtils.GetPrefabVariantParents(resolver.gameObject);
                var parentPaths = parents.Select(p => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(p.gameObject)).ToList();

                for (int i = 0; i < _styleChoices.Count; i++)
                {
                    if (parentPaths.Contains(_styleChoices[i].PrefabAssetPath))
                    {
                        currentStyle = _styleChoices[i].Name;
                        currentStylePrefabAssetPath = _styleChoices[i].PrefabAssetPath;
                        break;
                    }
                }
            }
            
            // Second we filter all styles and leave only those that this item can be converted to.
            var styleDropDown = element.Q<DropdownField>(className:"ui-entry-style");
            var choices = _styleChoices
                .Where( s => 
                {
                    // Check data type match.
                    if (s.SupportedDataTypes.Intersect(resolver.GetSupportedDataTypes()).IsNullOrEmpty())
                    {
                        return false;
                    }

                    // Check if prefabs can be converted.
                    if (!SettingsGeneratorPrefabStyleConverter.CanConvertPath(currentStylePrefabAssetPath, s.PrefabAssetPath))
                    {
                        return false;
                    }

                    // TODO: Once we support UI Toolkit then add a check for it here too.
                    
                    return true;
                } )
                .Select( s => s.Name).ToList();
            // Add "Custom" option only if the current one is a custom style (we can not convert to nor from it).
            if (currentStylePrefabAssetPath == null)
                choices.Insert(0, "Custom");
            styleDropDown.choices = choices;
            styleDropDown.SetValueWithoutNotify(currentStyle);
            styleDropDown.RegisterValueChangedCallback((e) => onStyleChanged(e, index, resolver) );

            var editBtn = element.Q<Button>(className:"ui-entry-edit");
            editBtn.userData = new Tuple<int, SettingResolver>(index, resolver); 
            editBtn.RegisterCallback<ClickEvent>(onEditEntryClicked);
            editBtn.text = "Edit";
            
            var resolverBtn = element.Q<Button>(className:"ui-entry-resolver");
            resolverBtn.userData = new Tuple<int, SettingResolver>(index, resolver); 
            resolverBtn.RegisterCallback<ClickEvent>(onResolverEntryClicked);
            
            // Clear details
            element.Q<VisualElement>(className:"ui-entry-details").Hide().Clear();
        }

        private void onStyleChanged(ChangeEvent<string> e, int index, SettingResolver resolver)
        {
            if (resolver == null || resolver.gameObject == null)
                return;
            
            if (e.previousValue == "Custom")
            {
                // Either "Custom" or a user generated prefab for which there is no conversion in SettingsGeneratorPrefabStyleConverter.
                // TODO: Support converting based only on the resolver (as long as it has a resolver it should be convertible).
                Logger.LogWarning("Sorry, you can not convert from custom.");
            }
            
            var styleChoice = _styleChoices.FirstOrDefault(s => s.Name == e.newValue);
            if (styleChoice != null)
            {
                styleChoice.ConvertFunc?.Invoke(resolver.gameObject);    
            }
            else
            {
                // Either "Custom" or a user generated prefab for which there is no conversion in SettingsGeneratorPrefabStyleConverter.
                Logger.LogWarning("Sorry, you can not convert to custom.");
            }
        }

        private void onEditEntryClicked(ClickEvent evt)
        {
            var ve = evt.target as VisualElement;
            var (index, resolver) = (Tuple<int, SettingResolver>) ve.userData;
                
            var detailsCtn = ve.FindFirstAncestorWithClass("ui-entry").Q<VisualElement>(className: "ui-entry-details");
            detailsCtn.ToggleDisplay();

            var editBtn = evt.target as Button;
            editBtn.text = detailsCtn.IsDisplayed() ? "Close" : "Edit";

            var entryCtn = detailsCtn.FindFirstAncestorWithClass("ui-entry");
            if (detailsCtn.IsDisplayed())
                entryCtn.AddToClassList("editing");
            else
                entryCtn.RemoveFromClassList("editing");

            if (detailsCtn.IsDisplayed() && detailsCtn.IsEmpty())
            {
                makeAndBindItemDetails(detailsCtn, index, resolver);
            }
        }
        
        private void onResolverEntryClicked(ClickEvent evt)
        {
            var ve = evt.target as VisualElement;
            var (_, resolver) = (Tuple<int, SettingResolver>) ve.userData;
            Selection.activeGameObject = resolver.gameObject;
        }
        
        private void onDeleteEntryClicked(ClickEvent evt)
        {
            var ve = evt.target as VisualElement;
            var resolver = ve.userData as SettingResolver;
            Undo.DestroyObjectImmediate(resolver.gameObject);
            updateResolverList();
        }
    }
}