#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Editor and runtime configuration of the settings system.
    /// If a provider is set then it will load the settings after Awake() but before Start() in the very first scene.
    /// </summary>
    public class SettingsGeneratorSettings : ScriptableObject
    {
        public const string Version = "1.55.0";
        public const string SettingsFilePath = "Assets/Resources/SettingsGenerator/SettingsGeneratorSettings.asset";
        public const string SettingsDirPath = "Assets/Resources/SettingsGenerator/";

        public const string _showEditorInfoLogsHint = ShowEditorInfoLogsHint; // <- Backwards compatibility.
        public const string ShowEditorInfoLogsHint = "You can turn this log message off in the settings (Tools > Settings Generator > Settings : Show Editor Info Logs).";
        
        
        [Header("Editor Settings")]
        
        [SerializeField, Tooltip("Turn off if you no longer want to see the 'Setting has no effect in the Editor. Please try in a build.' log messages.")]
        public bool ShowEditorInfoLogs = true;

        /// <summary>
        /// Don't use this at runtime. Use the 'provider' instead. It takes initializers into account.
        /// </summary>
        [Header("Runtime Settings")]
        [Tooltip("Sets the provider that will be used.\n" +
                 "NOTICE: If you have a SettingsInitializer in your very first loaded scene then that will be used instead. The examples use that technique to set the used provider.\n\n" +
                 "Do NOT use providers from the examples here. Those will be overwritten if you update the asset. You should create a new one (usually happens automatically).")]
        [SerializeField]
        public SettingsProvider DefaultProvider;
        public const string _DefaultProviderFieldName = "DefaultProvider";

        public bool HasDefaultProvider => DefaultProvider != null;

        /// <summary>
        /// If there is a provider set by a settings initializer then use that one. If not, then use the provider configured here.<br />
        /// Using initializer supported primarily for backwards compatibility reasons. In v1 we did not yet have a SettingsProvider field but only the initializer.< br />
        /// However, initializers are a nice way of setting the used provider on a per-scene bases (useful for demos) and thus we kept them.<br />
        /// In the editor during EDIT time it will always return the asset (initializers are ignored). 
        /// </summary>
        public SettingsProvider Provider
        {
            get
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return DefaultProvider;
                }
                else
                {
#endif
                    return SettingsInitializer.Instance != null ? SettingsInitializer.Instance.Provider : DefaultProvider;
#if UNITY_EDITOR    
                }
#endif
            }
        }

        [SerializeField, Tooltip("Any log above this log level will not be shown. To turn off all logs choose 'NoLogs'")]
        public Logger.LogLevel LogLevel = Logger.LogLevel.Warning;

#if UNITY_EDITOR
        [System.NonSerialized]
        protected SerializedObject _serializedObject;

        public SerializedObject SerializedObject
        {
            get
            {
                if (_serializedObject == null)
                {
                    _serializedObject = new SerializedObject(this);
                }

                return _serializedObject;
            }
        }
#endif
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void bindLoggerLevelToSetting()
        {
            // Notice: This does not yet create a setting instance because
            // it only creates a function but does not execute it!
            Logger.OnGetLogLevel = () => GetOrCreate().LogLevel;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void onAfterSceneLoadAtRuntime()
        {
            var settings = GetOrCreate();
            settings.initializeAtRuntime();

            SceneManager.sceneLoaded += GetOrCreate().onSceneLoaded;
        }

        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Provider == null)
                return;
            
            if (!Provider.ApplyOnSceneLoad)
                return;

            var existingApplier = SettingsApplier.GetApplier(scene);
            if (existingApplier == null && Provider != null)
            {
                var applier = SettingsApplier.CreateApplier(Provider, scene);
            }
        }

        // This is called after all Awake() but before all Start() methods.
        void initializeAtRuntime()
        {
            // Stop if initializer is used.
            if (SettingsInitializer.Exists)
                return;

            // If not valid initializer was found then use the one here.
            if (Provider != null)
            {
                // Pre init
                Provider.PreInitializationEvents?.Invoke();
                
                // Init
                var _ = Provider.Settings;
            }
            else
            {
                Logger.LogWarning("Could not load settings. Please set the 'SettingsProvider' on Resources/SettingsGenerator/SettingsGeneratorSettings or (legacy) add a SettingsInitializer to your scene.");
            }
        }

        static SettingsGeneratorSettings cachedConfig;
        
#if UNITY_EDITOR
        [DidReloadScripts(-1)]
        public static void onDomainReload()
        {
            EditorApplication.delayCall += () =>
            {
                GetOrCreate();
            };
        }
#endif

#if UNITY_EDITOR
        [System.NonSerialized]
        private static bool _triedToDeleteOldAsset = false;
#endif
        
        // Backwards compatibility
        public static SettingsGeneratorSettings GetOrCreateSettings() => GetOrCreate();
        
        public static SettingsGeneratorSettings GetOrCreate()
        {
#if UNITY_EDITOR
            
            // Delete old asset if it exists (do only one per recompile).
            if (!_triedToDeleteOldAsset)
            {
                _triedToDeleteOldAsset = true;
                AssetDatabase.DeleteAsset("Assets/SettingsGeneratorSettings.asset");
            }

            // In the editor we also check if the actual asset exists.
            if (cachedConfig != null && (EditorApplication.isPlayingOrWillChangePlaymode || AssetDatabase.GetAssetPath(cachedConfig) != null))
            {
                return cachedConfig;
            }
            
            cachedConfig = Resources.Load<SettingsGeneratorSettings>( SettingsFilePath.Replace("Assets/Resources/", "").Replace(".asset", ""));
            
            if (cachedConfig == null)
            {
                string dir = System.IO.Path.GetDirectoryName(SettingsFilePath);
                EditorRuntimeUtils.CreateFolder(dir);
                
                cachedConfig = ScriptableObject.CreateInstance<SettingsGeneratorSettings>();
                cachedConfig.ShowEditorInfoLogs = true;
                cachedConfig.DefaultProvider = null;
                cachedConfig.LogLevel = Logger.LogLevel.Warning;
        
                AssetDatabase.CreateAsset(cachedConfig, SettingsFilePath);
                
                return cachedConfig;
            }
#else
            if (cachedConfig != null)
                return cachedConfig;

            cachedConfig = Resources.Load<SettingsGeneratorSettings>( SettingsFilePath.Replace("Assets/Resources/", "").Replace(".asset", ""));
#endif
            return cachedConfig;
            
        }

#if UNITY_EDITOR
        // We use this callback instead of CompilationPipeline.compilationFinished because
        // compilationFinished runs before the assembly has been reloaded but DidReloadScripts
        // runs after. And only after we can access the Settings asset.
        [UnityEditor.Callbacks.DidReloadScripts(999000)]
        public static void DidReloadScripts()
        {
            bool versionChanged = VersionHelper.UpgradeVersion(getVersionFunc: AssetInfos.GetVersion, out var oldVersion, out var newVersion);
            if (versionChanged)
            {
                Debug.Log("VERSION CHANGED to " + newVersion);
                AssemblyDefinitionUpdater.CheckAndUpdate();
            }
        }

        [MenuItem("Tools/Settings Generator/Manual", priority = 101)]
        public static void OpenManual()
        {
            Application.OpenURL(Installer.ManualUrl);
        }

        [MenuItem("Tools/Settings Generator/Open Example Scene", priority = 103)]
        public static void OpenExample()
        {
            string path = "Assets/Kamgam/SettingsGenerator/Examples/FromAsset/SettingsFromAssetDemo.unity";
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            EditorGUIUtility.PingObject(scene);
            EditorSceneManager.OpenScene(path);
        }

        [MenuItem("Tools/Settings Generator/Configuration", priority = 100)]
        public static void OpenSettings()
        {
            var settings = SettingsGeneratorSettings.GetOrCreate();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Settings Generator Settings could not be found or created.", "Ok");
            }
        }

        [MenuItem("Tools/Settings Generator/Please leave a review :-)", priority = 510)]
        public static void LeaveReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/240015?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/Settings Generator/More Asset by KAMGAM", priority = 511)]
        public static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/37829?aid=1100lqC54&pubref=asset");
        }

        [MenuItem("Tools/Settings Generator/Version " + Version, priority = 512)]
        public static void LogVersion()
        {
            Debug.Log("Settings Generator v" + Version);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SettingsGeneratorSettings))]
    public class SettingsGeneratorSettingsEditor : Editor
    {
        public SettingsGeneratorSettings settings;

        public void OnEnable()
        {
            settings = target as SettingsGeneratorSettings;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Version: " + SettingsGeneratorSettings.Version);
            base.OnInspectorGUI();
        }
    }
#endif
}