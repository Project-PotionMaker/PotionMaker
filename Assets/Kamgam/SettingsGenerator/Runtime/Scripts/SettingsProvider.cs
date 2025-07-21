#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// The Settings Provider creates an instance of the Settings object
    /// and keeps a reference to it so other objects can come and ask
    /// for it at any time.
    /// <br /><br />
    /// It also handles resetting objects before play mode
    /// if Domain-Reload is disabled (via IResetBeforeDomainReload).
    /// </summary>
    [CreateAssetMenu(fileName = "SettingsProvider", menuName = "SettingsGenerator/SettingsProvider", order = 1)]
    public class SettingsProvider : ScriptableObject, ISerializationCallbackReceiver
#if UNITY_EDITOR
        , IResetBeforeDomainReload
#endif
    {
        /// <summary>
        /// Hold a reference to the last used SettingsProvider.<br />
        /// You should NOT build your code upon this, it may be null (especially before initialization).<br />
        /// However it can be very handy if you know that you are only using one single provider and you need to fetch it quickly.
        /// </summary>
        public static SettingsProvider LastUsedSettingsProvider;

        [Header("Storage")]
        [SerializeField, Tooltip("The player prefs key (or file name) under which your settings will be saved.\n\n" +
                                 "It is still named 'PlayerPrefs..' for backwards compatibility reasons but if you save as json is the filename (within persistent data path) without the extension.")]
        protected string playerPrefsKey;
        
        [Tooltip("Hee  you can choose how the settings will be saved. Currently Prefabs and JSON is supported but you can add your own method by creating a new ScriptableObject derived from SettingsSaverBase. If null then 'Prefabs' will be used as fallback.")]
        public SettingsSaverBase SettingsSaver = null;

        [Tooltip("The default settings asset.\nYou can leave this empty if you define all your settings via script.")]
        [FormerlySerializedAs("Default")]
        public Settings SettingsAsset;

        /// <summary>
        /// Creates a settings asset copy for runtime use if needed. Though it does NOT load it yet.
        /// Use this to modify the settings in Awake before any settings are loaded (handy for script-only
        /// settings for example).
        /// </summary>
        /// <returns></returns>
        public Settings GetOrCreateRuntimeSettingsAsset()
        {
            if (_settings == null)
            {
                // Create a new
                if (SettingsAsset == null)
                {
                    // Create a new instance from code.
                    _settings = ScriptableObject.CreateInstance<Settings>();
                }
                else
                {
                    // Create a copy from asset for runtime use.
                    _settings = ScriptableObject.Instantiate(SettingsAsset);
                }
            }

            return _settings;
        }

        /// <summary>
        /// Used to check whether an initial load is necessary if the settings asset runtime copy already exists.
        /// Usually it does not but it might if it was modified by the user via code. 
        /// </summary>
        [System.NonSerialized]
        protected bool _initialLoadDone;

        public bool InitialLoadDone => _initialLoadDone;
        
        protected Settings _settings;
        public Settings Settings
        {
            get
            {
                LastUsedSettingsProvider = this;

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                    return null;
#endif

                if (_settings == null || !_initialLoadDone)
                {
                    // Create asset copy for runtime use
                    _settings = GetOrCreateRuntimeSettingsAsset();
                
                    // Make a global backup copy of the current quality level (we use this in the Connections to restore it later).
                    QualityPresets.AddCurrentLevel();

                    // Load user settings from storage
                    _settings.Load(playerPrefsKey, SettingsSaver, RemoveUnknownSettingsAfterLoad);

                    // Register to setting changed for auto-loading
                    _settings.OnSettingChanged += onSettingChanged;
                    
                    // Mark as initially loaded.
                    _initialLoadDone = true;
                }

                return _settings;
            }
        }

        /// <summary>
        /// At runtime it returns the settings copy (same as .Settings) but at edit time it returns the SettingsAsset.<br />
        /// This is handy if you need to get the settings in both edit and runtime.
        /// </summary>
        /// <returns></returns>
        public Settings GetSettingsAssetOrRuntimeCopy()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
                return Settings;
            else
                return SettingsAsset;
#else
            return Settings;
#endif
        }
        
        [Header("Initialization")]
        
        [Tooltip("Should settings that are not in the Settings Asset list (but are still in the stored data on disk) be removed after loading?\n" +
                 "NOTICE: If enabled then settings that are added via code need to be added before loading (otherwise those would be removed too).")]
        public bool RemoveUnknownSettingsAfterLoad = false;
        
        [Tooltip("Use this to register event methods that should be executed BEFORE the settings are initialized.")]
        public UnityEvent PreInitializationEvents;

        [Header("Auto Save")]
        
        /// <summary>
        /// If turned on then for each change in a setting a save will be SCHEDULED. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.
        /// </summary>
        [Tooltip("If turned on then for each change in a setting a save will be SCHEDULED. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.")]
        public bool AutoSave = true;
        
        /// <summary>
        /// Only used if AutoSave is turned on. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.
        /// </summary>
        [Tooltip("Only used if AutoSave is turned on. If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.")]
        public float AutoSaveWaitTimeInSec = 1.0f;
        
        [Tooltip("Should the settings be saved once the UI is closed (all resolvers are inactive)?")]
        public bool AutoSaveOnClose = true;
        
        [Tooltip("Should the settings be saved once the application is being closed?")]
        public bool AutoSaveOnQuit = false;
        
        public enum UnappliedOnCloseBehaviour { Ignore, Revert, Apply, TriggerCheckForUnappliedInScene }

        
        [Header("Apply")]
        [Tooltip("Defines what to do with unapplied settings once the UI is closed (all resolvers are inactive)?\n" +
                 "If set to TriggerCheckForUnappliedInScene then you should have a SettingsCheckForUnapplied component in your scene. It will trigger the check on it.\n\n" +
                 "NOTICE: If you have SettingsCheckForUnapplied components in your scene and selected 'Ignore' here (the default) then those in the scene will still be executed as before (backwards compatibility).")]
        public UnappliedOnCloseBehaviour UnappliedBehaviourOnClose = UnappliedOnCloseBehaviour.Ignore;
        
        /// <summary>
        /// Event parameter is the list of unapplied settings.
        /// </summary>
        public UnityEvent<List<ISetting>> OnUnappliedOnClose;
        
       
        [Header("On Scene Load")]

        [Tooltip("If enabled then you can remove any setting appliers you have because it will automatically create on in each new loaded scene.\n" +
                 "NOTICE: If you still have SettingsAppliers in your scene then this will do nothing (your existing applier will take precedence). This ensures backwards compatibility.")]
        public bool ApplyOnSceneLoad = true;
        
        [Tooltip("On start delay in seconds.")]
        public float ApplyOnSceneLoadDelay = 0f;
        
        [Tooltip("Only use this as a last resort if another system keeps overriding your settings.\n" +
                 "You really should find out what system that is and route the settings through that instead of using this.")]
        public bool ApplyOnSceneLoadInLateUpdate = false;

        [Tooltip("Leave empty to apply all settings. If set then only these setting ids will be applied on scene load.")]
        public List<string> ApplyOnSceneLoadIds = new List<string>();

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

#region Initialize Field
        [SerializeField, HideInInspector]
        protected int initializedVersion;
        
        // We use serialization callbacks to init the default values.
        public void OnBeforeSerialize() {}

        // Used to trigger init on old object upon deserialization.
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            Initialize();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Initializes new field. Can be called multiple times.
        /// </summary>
        public void Initialize()
        {
            bool modified = false;
            
            if (initializedVersion < 2)
            {
                initializedVersion = 2;
                modified = true;
                
                ApplyOnSceneLoad = true;
                ApplyOnSceneLoadDelay = 0f;
                ApplyOnSceneLoadInLateUpdate = false;
                ApplyOnSceneLoadIds = new List<string>();
            }

            if (initializedVersion < 3)
            {
                initializedVersion = 3;
                modified = true;
                
                AutoSaveOnClose = true;
                UnappliedBehaviourOnClose = UnappliedOnCloseBehaviour.Ignore;
            }
            
            if (initializedVersion < 4)
            {
                initializedVersion = 4;
                modified = true;
                
                AutoSaveOnQuit = true;
            }

            if (modified)
            {
                // Save changes
                EditorApplication.delayCall += () =>
                {
                    EditorUtility.SetDirty(this);
#if UNITY_2021_2_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(this); 
#else
                    AssetDatabase.SaveAssets();
#endif
                };
            }
        }
#endif
#endregion
        
        /// <summary>
        /// Use this to check whether or not the settings have loaded.
        /// </summary>
        public bool HasSettings()
        {
            return _settings != null;
        }

#pragma warning disable CS0414
        [SerializeField, HideInInspector] private bool _hasBeenInitialisedInEditor = false;
#pragma warning restore  CS0414
        [System.NonSerialized] private double _awakeTime;

#if UNITY_EDITOR
        private void Awake()
        {
            if (!_hasBeenInitialisedInEditor)
            {
                _hasBeenInitialisedInEditor = true;
                EditorUtility.SetDirty(this);

                _awakeTime = EditorApplication.timeSinceStartup;
                UnityEditor.EditorApplication.update += waitForValidPath;
            }
            
#if UNITY_EDITOR
            assignSettingsSaverIfNeeded();
#endif
        }

        private void waitForValidPath()
        {
            // Delay for 0.5 Sek to give the deserialization time to load the settings asset.
            // TODO: Add a logic fix, not a timing based fix. Actually with "_hasBeenInitialisedInEditor"
            //       this should no longer be needed. Investigate and remove.
            if (EditorApplication.timeSinceStartup - _awakeTime > 0.5f)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(this);

                // Wait until the path is ready
                if (!string.IsNullOrEmpty(path))
                {
                    UnityEditor.EditorApplication.update -= waitForValidPath;

                    EditorCreateNewSettingsInstance();
                }
            }
        }

        static string getSanitizedProductName()
        {
            return System.Text.RegularExpressions.Regex.Replace(Application.productName, @"[^-a-zA-Z0-9._ ]", "");
        }

        public void EditorCreateNewSettingsInstance()
        {
            // Rename provider to product name.
            var providerPath = AssetDatabase.GetAssetPath(this);
            if (providerPath.EndsWith("SettingsProvider.asset"))
            {
                string sanitizedProductName = getSanitizedProductName();
                string newName = name + " (" + sanitizedProductName + ")";
                AssetDatabase.RenameAsset(providerPath, newName);
                name = newName;
            }

            // Upgrading (special calse: do NOT propose settings creation for code demo provider).
            if (playerPrefsKey == "SGSettings_Code")
            {
                return;
            }

            if (SettingsAsset != null)
            {
                return;
            }

            bool createSettings = EditorUtility.DisplayDialog(
                "Create Settings list for '" + this.name + "'?",
                "It seems this is a new SettingsProvider.\n\n" +
                "Would you like to create a settings list for it?"
                , "Yes (recommended)", "No");
            if (createSettings)
            {
                Logger.LogMessage("SettingsProvider found: " + providerPath);

                // Auto create settings file for provider.
                var settingsPath = System.IO.Path.GetDirectoryName(providerPath).Replace("\\", "/") + "/" + name.Replace("Provider", "") + ".asset";
                if (settingsPath == providerPath)
                    settingsPath = settingsPath.Replace(".asset", " Settings.asset");

                // Create based on template
                bool createdFromTemplate = false;
                string templatePath = "";
                Settings settings = null;
                var guids = AssetDatabase.FindAssets("t:Settings \"Settings (Template)\"");
                if (guids.Length > 0)
                {
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var settingsTemplate = AssetDatabase.LoadAssetAtPath<Settings>(path);
                        if (settingsTemplate != null)
                        {
                            settings = ScriptableObject.Instantiate(settingsTemplate);

                            // Set all settings disabled.
                            settings.RebuildSettingsCache();
                            foreach (var setting in settings.GetAllSettings())
                            {
                                setting.IsActive = false;
                            }

                            createdFromTemplate = true;
                            templatePath = path;
                            break;
                        }
                    }
                }

                // If template was not used then create an empty settings list.
                if (settings == null)
                {
                    settings = Settings.CreateInstance<Settings>();
                    settings.GetOrCreateFloat("dummy", 1f);
                }

                AssetDatabase.CreateAsset(settings, settingsPath);
                SettingsAsset = settings;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);

                EditorGUIUtility.PingObject(this);
                EditorGUIUtility.PingObject(SettingsAsset);
                Selection.objects = new Object[] { SettingsAsset };

                if (createdFromTemplate)
                    Logger.LogMessage("Settings created based on '" + templatePath + "' under: " + settingsPath);
                else
                    Logger.LogMessage("Settings created under: " + settingsPath);
            }
        }
#endif

        private string getDefaultStorageKey()
        {
            string newPlayerPrefsKey = "Settings." + System.Text.RegularExpressions.Regex.Replace(Application.productName, @"[^-a-zA-Z0-9_]", "");
            return newPlayerPrefsKey;
        }

        public void OnEnable()
        {
            if (string.IsNullOrEmpty(playerPrefsKey))
            {
                playerPrefsKey = getDefaultStorageKey();
            }

#if UNITY_EDITOR
            assignSettingsSaverIfNeeded();
#endif
            if (SettingsSaver == null)
            {
                SettingsSaver = CreateInstance<SettingsSaverPlayerPrefs>();
            }
        }

#if UNITY_EDITOR
        void assignSettingsSaverIfNeeded()
        {
            if (SettingsSaver == null)
            {
                var guids = AssetDatabase.FindAssets("t:SettingsSaverPlayerPrefs");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var saver = AssetDatabase.LoadAssetAtPath<SettingsSaverPlayerPrefs>(path);
                    SettingsSaver = saver;

                    EditorApplication.delayCall += () =>
                    {
                        EditorUtility.SetDirty(this);
                        SaveAssetHelper.SaveAssetIfDirty(this);
                    };
                }
            }
        }
#endif

#if UNITY_EDITOR
        // Domain Reload handling
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void onResetBeforePlayMode()
        {
            DomainReloadUtils.CallOnResetOnAssets(typeof(SettingsProvider));
        }

        public void ResetBeforePlayMode()
        {
            _settings = null;
        }
#endif

        /// <summary>
        /// Resets all settings to their DEFAULT values.
        /// TODO: In future versions rename to ResetToDefaults().
        /// </summary>
        public void Reset()
        {
            if (Settings != null)
                Settings.Reset();
            
#if UNITY_EDITOR
            Initialize();
#endif
        }

        public void Reset(params string[] ids)
        {
            Settings.Reset(ids);
        }

        public void ResetGroups(params string[] groups)
        {
            Settings.ResetGroups(groups);
        }

        public void ResetGroup(string group)
        {
            Settings.ResetGroups(group);
        }
        
        public void ResetToUnappliedValues()
        {
            ResetToUnappliedValues(propagateChange: true);
        }
        
        public void ResetToUnappliedValues(bool propagateChange)
        {
            Settings.ResetToUnappliedValues(propagateChange);
        }

        /// <summary>
        /// Applies the settings.<br />
        /// This means that if a setting has a connection it will be pushed and then pulled.
        /// </summary>
        public void Apply()
        {
            Apply(changedOnly: true, triggerChangeEvents: false);
        }
        
        /// <summary>
        /// Applies the settings.<br />
        /// This means that if a setting has a connection it will be pushed and then pulled.
        /// </summary>
        /// <param name="changedOnly">Apply only those which have changed.</param>
        public void Apply(bool changedOnly)
        {
            Settings?.Apply(changedOnly, triggerChangeEvents: false);
        }
        
        /// <summary>
        /// Applies the settings.<br />
        /// This means that if a setting has a connection it will be pushed and then pulled.
        /// </summary>
        /// <param name="changedOnly">Apply only those which have changed.</param>
        /// <param name="triggerChangeEvents">Since apply usually only triggers a push to connections you can use this to also trigger the change event on all settings without connections.</param>
        public void Apply(bool changedOnly, bool triggerChangeEvents)
        {
            Settings?.Apply(changedOnly, triggerChangeEvents);
        }


        // Load & Save

        public void Load()
        {
            if (_settings == null)
            {
                // At the very first load this will be executed.

                // Accessing the "Settings" getter for the very first time
                // causes a load automatically, thus we do not need to load
                // anything here.
                Settings.RefreshRegisteredResolvers();
            }
            else
            {
                // Pull values from connections to initialize the default values.
                Settings.PullFromConnections();

                // Load user settings from storage
                // Also triggers resolver updates (aka Settings.RefreshRegisteredResolvers())
                Settings.Load(playerPrefsKey, SettingsSaver);
            }
        }

        public void ResetToLastSave()
        {
            // Load user settings from storage
            // Also triggers resolver updates (aka Settings.RefreshRegisteredResolvers())
            Settings.Load(playerPrefsKey, SettingsSaver);
        }

        public void Save()
        {
            Settings?.Save(playerPrefsKey, SettingsSaver);
        }

        public void Delete()
        {
            if (Settings != null)
            {
                Settings.Delete(playerPrefsKey, SettingsSaver);
            }
            else
            {
                // This only exists to support deleting in Editor if not in play mode.
                // Notice the static use of Settings.
                Settings.DeletePlayerPrefs(playerPrefsKey);
            }
        }

        // Auto Save

        protected void onSettingChanged(ISetting setting)
        {
            if (AutoSave)
            {
                ScheduleAutoSave(AutoSaveWaitTimeInSec);
            }
        }

        [System.NonSerialized]
        protected float _autoSaveTime = -1f;

        /// <summary>
        /// If for AutoSaveWaitTimeInSec after the last change no further change happens then it will save.
        /// </summary>
        public void ScheduleAutoSave(float autoSaveWaitTimeInSec)
        {
            if (_autoSaveTime < 0f)
            {
                _autoSaveTime = Time.realtimeSinceStartup + autoSaveWaitTimeInSec;
                scheduleAutoSaveAsync();
            }
            else
            {
                _autoSaveTime = Time.realtimeSinceStartup + autoSaveWaitTimeInSec;
            }
        }

        protected async void scheduleAutoSaveAsync()
        {
            // Wait for the timer to run out.
            float deltaTime = _autoSaveTime - Time.realtimeSinceStartup;
            while (deltaTime > 0)
            {
                await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(deltaTime * 1000) + 50);
                deltaTime = _autoSaveTime - Time.realtimeSinceStartup;
            }

            Save();
            _autoSaveTime = -1f;
        }

        private static List<ISetting> s_tmpListOfUnappliedSetings = new List<ISetting>();

        public void OnAllResolversDeactivated(bool isQuitting)
        {
            if (Settings == null)
                return;
            
            if (AutoSaveOnQuit || !isQuitting)
            {
                Save();
            }

            if (UnappliedBehaviourOnClose == UnappliedOnCloseBehaviour.Apply)
            {
                Settings.Apply(changedOnly: true);
            }
            else if(UnappliedBehaviourOnClose == UnappliedOnCloseBehaviour.Revert)
            {
                Settings.ResetToUnappliedValues();
            }
            else if(UnappliedBehaviourOnClose == UnappliedOnCloseBehaviour.TriggerCheckForUnappliedInScene)
            {
                SettingsCheckForUnapplied.TriggerCheck();
            }
            
            if ( OnUnappliedOnClose != null)
            {
                Settings.GetUnappliedSettings(s_tmpListOfUnappliedSetings);
                OnUnappliedOnClose.Invoke(s_tmpListOfUnappliedSetings);
                s_tmpListOfUnappliedSetings.Clear();
            }
        }
        
        
        
        
        

        // Static Helpers in Editor
#if UNITY_EDITOR
        public bool EditorIsExampleProvider()
        {
            var path = AssetDatabase.GetAssetPath(this);
            return path.Contains("Kamgam"); // Notice: this is used in other code parts below too.
        }
        
        /// <summary>
        /// Returns a list of used providers in all loaded scenes. Usually that list contains only one entry since all settings should use the same provider.
        /// </summary>
        /// <param name="excludeExampleProviders"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<SettingsProvider> EditorFindAllProvidersInLoadedScenes(bool excludeExampleProviders)
        {
            var providers = new System.Collections.Generic.List<SettingsProvider>();

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    var resolvers = root.GetComponentsInChildren<ISettingResolver>(includeInactive: true);
                    foreach (var resolver in resolvers)
                    {
                        if (resolver != null && resolver.GetProvider() != null && !providers.Contains(resolver.GetProvider()))
                        {
                            if (excludeExampleProviders && resolver.GetProvider().EditorIsExampleProvider())
                                continue;
                            
                            providers.Add(resolver.GetProvider());
                        }
                    }
                    
                    var appliers = root.GetComponentsInChildren<SettingsApplier>(includeInactive: true);
                    foreach (var applier in appliers)
                    {
                        if (applier != null && applier.Provider != null && !providers.Contains(applier.Provider))
                        {
                            if (excludeExampleProviders && applier.Provider.EditorIsExampleProvider())
                                continue;
                            
                            providers.Add(applier.Provider);
                        }
                    }
                    
                    var initializers = root.GetComponentsInChildren<SettingsInitializer>(includeInactive: true);
                    foreach (var initializer in initializers)
                    {
                        if (initializer != null && initializer.Provider != null && !providers.Contains(initializer.Provider))
                        {
                            if (excludeExampleProviders && initializer.Provider.EditorIsExampleProvider())
                                continue;
                            
                            providers.Add(initializer.Provider);
                        }
                    }
                }
            }
            
            return providers;
        }
        
        public static System.Collections.Generic.List<SettingsProvider> EditorFindAllProviders(bool excludeExampleProviders, bool limitToDefaultResources = false)
        {
            var providers = new System.Collections.Generic.List<SettingsProvider>();

            var guids = AssetDatabase.FindAssets("t:" + typeof(SettingsProvider).Name);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (excludeExampleProviders && path.Contains("Kamgam"))
                    continue;

                if (limitToDefaultResources && !path.Contains("Assets/Resources"))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<SettingsProvider>(path);
                if (asset != null)
                    providers.Add(asset);
            }

            return providers;
        }

        public static SettingsProvider EditorFindTemplate()
        {
            var providers = EditorFindAllProviders(excludeExampleProviders: false);
            foreach (var provider in providers)
            {
                if (provider.name.Contains("Template"))
                    return provider;
            }

            return null;
        }

        /// <summary>
        /// Creates a new settings provider asset based on the template.
        /// </summary>
        /// <param name="providerPath">Folder name that will be created. Path can start with Assets/ or not.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SettingsProvider EditorCreateProviderBasedOnTemplate(string providerPath, string name = null, bool pingAfterCreation = false)
        {
            providerPath = EditorRuntimeUtils.makePathRelativeToAssets(providerPath);
            var template = EditorFindTemplate();
            if (template != null)
            {
                // Create dir
                EditorRuntimeUtils.CreateFolder(providerPath);

                // Create name
                string sanitizedProductName = getSanitizedProductName();
                string newName = name;
                if (string.IsNullOrEmpty(newName))
                {
                    newName = "SettingsProvider (" + sanitizedProductName + ")";
                }

                // Create provider
                var provider = SettingsProvider.Instantiate(template);
                provider.name = newName;
                provider.playerPrefsKey = provider.getDefaultStorageKey();

                // Create Settings object
                var settingsTemplate = template.SettingsAsset;
                var settings = Settings.Instantiate(settingsTemplate);
                provider.SettingsAsset = settings;
                settings.name = provider.name.Replace("Provider", "");
                // Set all settings disabled.
                settings.RebuildSettingsCache();
                foreach (var setting in settings.GetAllSettings())
                {
                    setting.IsActive = false;
                }

                // Persist assets
                AssetDatabase.CreateAsset(provider, "Assets/" + providerPath + "/" + provider.name + ".asset");
                AssetDatabase.CreateAsset(settings, "Assets/" + providerPath + "/" + settings.name + ".asset");

                if (pingAfterCreation)
                {
                    EditorGUIUtility.PingObject(provider);
                }

                return provider;
            }

            return null;
        }

        public static bool DoesCustomProviderExist()
        {
            var providers = Kamgam.SettingsGenerator.SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true);
            return providers.Count > 0;
        }

        public static SettingsProvider GetFirstCustomProvider()
        {
            var providers = Kamgam.SettingsGenerator.SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true);
            foreach (var provider in providers)
            {
                string path = AssetDatabase.GetAssetPath(provider);
                if (!path.Contains("Kamgam/SettingsGenerator/"))
                    return provider;
            }

            return null;
        }
        
        /// <summary>
        /// If a container is set the only the ui within that container is updated.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="container"></param>
        public static void EditorSetProviderInScene(SettingsProvider provider, GameObject container = null)
        {
            if (provider == null)
            {
                Logger.LogMessage($"Doing nothing because provider is null.");
                return;
            }

            bool didUpdate = false;
            
            // Update Resolvers
            var resolvers = SettingResolver.EditorFindResolversInLoadedScenes(includeInactive: true);
            foreach (var resolver in resolvers)
            {
                var resolverComp = resolver as MonoBehaviour;
                
                // Skip if not in container
                if (resolverComp != null && container != null && !resolverComp.transform.IsChildOf(container.transform))
                {
                    continue;
                }
                
                if (resolver.GetProvider() != provider)
                {
                    Logger.LogMessage($"Changing provider from '{resolver.GetProvider()}' to '{resolverComp.gameObject.name}' on '{resolverComp.name}'.", resolverComp.gameObject);
                    resolver.SetProvider(provider);
                    didUpdate = true;

                    // Make sure the scene can be saved
                    EditorUtility.SetDirty(resolverComp);
                    if (resolverComp.gameObject.scene != null)
                        EditorSceneManager.MarkSceneDirty(resolverComp.gameObject.scene);
                }

                if (provider.SettingsAsset != null && !provider.SettingsAsset.HasActiveID(resolver.GetID()) && provider.SettingsAsset.HasID(resolver.GetID()) )
                {
                    provider.SettingsAsset.SetActive(resolver.GetID(), true);
                    Logger.LogMessage("Activated inactive setting on '" + resolverComp.gameObject.name + "'. Please check if you really want this activated (click on this message to go to the object).", resolverComp.gameObject);
                }

                // Make sure the Prefab recognizes the changes
                PrefabUtility.RecordPrefabInstancePropertyModifications(resolverComp);

                EditorUtility.SetDirty(provider.SettingsAsset);
                AssetDatabase.SaveAssetIfDirty(provider.SettingsAsset);
            }

            // Update Initializers
            var initializers = CompatibilityUtils.FindObjectsOfType<SettingsInitializer>(includeInactive: true);
            foreach (var initializer in initializers)
            {
                if (initializer.Provider == provider)
                    continue;

                Logger.LogMessage("Setting Provider on " + initializer.gameObject.name);
                initializer.Provider = provider;
                didUpdate = true;

                // Make sure the scene can be saved
                EditorUtility.SetDirty(initializer);
                if (initializer.gameObject.scene != null)
                    EditorSceneManager.MarkSceneDirty(initializer.gameObject.scene);

                // Make sure the Prefab recognizes the changes
                PrefabUtility.RecordPrefabInstancePropertyModifications(initializer);
            }

            // Update Appliers
            var appliers = CompatibilityUtils.FindObjectsOfType<SettingsApplier>(includeInactive: true);
            foreach (var applier in appliers)
            {
                if (applier.Provider == provider)
                    continue;

                Logger.LogMessage("Setting Provider on " + applier.gameObject.name);
                applier.Provider = provider;
                didUpdate = true;

                // Make sure the scene can be saved
                EditorUtility.SetDirty(applier);
                if (applier.gameObject.scene != null)
                    EditorSceneManager.MarkSceneDirty(applier.gameObject.scene);

                // Make sure the Prefab recognizes the changes
                PrefabUtility.RecordPrefabInstancePropertyModifications(applier);
            }
            
            // Update AudioSourceVolumeConnectionComponent
            var audioSourceVolumeComponents = CompatibilityUtils.FindObjectsOfType<AudioSourceVolumeConnectionComponent>(includeInactive: true);
            foreach (var comp in audioSourceVolumeComponents)
            {
                if (comp.SettingsProvider == provider)
                    continue;

                Logger.LogMessage("Setting Provider on " + comp.gameObject.name);
                comp.SettingsProvider = provider;
                didUpdate = true;

                // Make sure the scene can be saved
                EditorUtility.SetDirty(comp);
                if (comp.gameObject.scene != null)
                    EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);

                // Make sure the Prefab recognizes the changes
                PrefabUtility.RecordPrefabInstancePropertyModifications(comp);
            }
            
            // Update SettingReceiverGenericConnector
            var genericReceivers = CompatibilityUtils.FindObjectsOfType<SettingReceiverGenericConnector>(includeInactive: true);
            foreach (var comp in genericReceivers)
            {
                if (comp.SettingsProvider == provider)
                    continue;

                Logger.LogMessage("Setting Provider on " + comp.gameObject.name);
                comp.SettingsProvider = provider;
                didUpdate = true;

                // Make sure the scene can be saved
                EditorUtility.SetDirty(comp);
                if (comp.gameObject.scene != null)
                    EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);

                // Make sure the Prefab recognizes the changes
                PrefabUtility.RecordPrefabInstancePropertyModifications(comp);
            }


            UpdateProvidersOnUnityEvents(provider);

            // EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            if (didUpdate)
            {
                Logger.LogMessage("Updated scene with provider '" + AssetDatabase.GetAssetPath(provider) + "'");
            }
        }
        
        public static void UpdateProvidersOnUnityEvents(SettingsProvider provider)
        {
            var behaviours = CompatibilityUtils.FindObjectsOfType<MonoBehaviour>(includeInactive: true);
            var eventType = typeof(UnityEvent);
            var providerType = provider.GetType();

            foreach (var behaviour in behaviours)
            {
                FieldInfo[] fields = behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (eventType.IsAssignableFrom(field.FieldType))
                    {
                        if (field.GetValue(behaviour) is UnityEvent unityEvent)
                        {
                            // Go through all events
                            List<int> eventsToModify = new List<int>();
                            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                            {
                                var target = unityEvent.GetPersistentTarget(i);
                                if (target != null && target.GetType() == typeof(SettingsProvider))
                                {
                                    // INFO: Tried to automate this but did not quite get there. TODO: Investigate later.

                                    // var methodName = unityEvent.GetPersistentMethodName(i);
                                    // // TODO: Check for ambiguous methods and select the correct one by parameter types.
                                    // MethodInfo methodInfo = providerType.GetMethod(methodName);
                                    // var methodParameters = methodInfo.GetParameters();
                                    // var delegateType = typeof(UnityAction<>).MakeGenericType(Array.ConvertAll(methodParameters, p => p.ParameterType));
                                    // var methodDelegate = System.Delegate.CreateDelegate(delegateType, provider, methodInfo);
                                    // // Thanks to Unity this method is internal ... *sigh*
                                    // // Parameters: int index, object targetObj, Type targetObjType, MethodInfo method
                                    // Type[] registerMethodTypes = new Type[] { typeof(int), typeof(object), typeof(Type), typeof(MethodInfo) };
                                    // var registerMethod = eventType.GetMethod("RegisterPersistentListener", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, registerMethodTypes, null);
                                    // if (registerMethod != null)
                                    // {
                                    //     // Always returns a "method could not be found." error. TODO: investigate.
                                    //     registerMethod.Invoke(unityEvent, new object[] { i, provider, providerType, methodInfo });
                                    // }

                                    // For now we only log a message to the user.
                                    if (target != provider)
                                    {
                                        Logger.LogWarning("Found a different provider used in a UnityEvent on " + behaviour.gameObject.name + ". Please update it (click on this message to go to the object).", behaviour.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
#endif
       
    }
}
