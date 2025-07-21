using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Kamgam.SettingsGenerator
{
    public partial class CreateSettingUGUIWindow : EditorWindow
    {
        public SettingsGeneratorSettings Config => SettingsGeneratorSettings.GetOrCreate();

        protected VisualElement _providerWelcomeCtn;
        protected VisualElement _providerInspectorCtn;
        protected VisualElement _settingsInspectorCtn;

        // Remembers the last provider before a demo scene was entered (used to reset if a non-demo scene is opened). 
        [System.NonSerialized]
        protected SettingsProvider _providerBeforeDemoScene = null;

        private void onShowChooseProvider(ClickEvent evt)
        {
            _serializedWindow.Update();
            Config.SerializedObject.Update();
            if (Config.DefaultProvider != null)
                Config.DefaultProvider.SerializedObject.Update();
            
            // Handle case of retuning from a demo scene to a regular scene.
            if (this.isDemoScene())
            {
                var resolvers = SettingResolver.FindResolversInLoadedScenes(includeInactive: true);
                if (resolvers.Count > 0)
                    Config.DefaultProvider = resolvers[0].GetProvider();
                else
                    Config.DefaultProvider = null;
            }
            else
            {
                // Reset if necessary.
                if (Config.DefaultProvider == null || AssetDatabase.GetAssetPath(Config.DefaultProvider).Contains("Kamgam"))
                {
                    Config.DefaultProvider = _providerBeforeDemoScene;
                    _providerBeforeDemoScene = null;
                }
            }
            
            getState(
                out var hasCustomProvider,
                out var hasDefaultProvider,
                out var isDemoScene,
                out var providersInScene,
                out var noProviderInSceneButDefaultExists,
                out var conflictBetweenDefaultAndScene,
                out _);
            
            // Handle settings provider cases

            // No event means it is coming from the initial screen. Which means we can skip it if possible
            if (noProviderInSceneButDefaultExists && evt == null)
            {
                // New: Do nothing since we not also show the settings here.
                // SKip to next step.
                // onShowChooseSetting(null);
                // return;
            }

            // Resolve provider conflict
            if (conflictBetweenDefaultAndScene && !isDemoScene)
            {
                bool fix = EditorUtility.DisplayDialog(
                        "Conflicting Settings Configuration",
                        $"Some objects in the scene use different settings ('{providersInScene[0].name}') than the chosen default ('{Config.Provider.name}').\n\n"+
                        $"Would you like to update them to the default '{Config.Provider.name}'?",
                        "Yes (recommended)", "No");
                if (fix)
                {
                    SettingsProvider.EditorSetProviderInScene(Config.DefaultProvider);
                }
            }

            bool createdNewAssets = false;
            if (!hasDefaultProvider && providersInScene.Count == 0)
            {
                // Take the first of the custom providers.
                if (hasCustomProvider)
                {
                    var customProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true, limitToDefaultResources: true);
                    if (customProviders.Count == 0)
                        customProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true, limitToDefaultResources: false);
                    
                    if (customProviders.Count > 0 && customProviders[0] != null)
                    {
                        Config.DefaultProvider = customProviders[0];
                        SettingsProvider.LastUsedSettingsProvider = Config.DefaultProvider;
                        Logger.LogMessage("Found custom settings provider under '" + AssetDatabase.GetAssetPath(customProviders[0]) + "'. If you want to use different ones then please change the 'Default Provider' field.");
                    }
                }
                else
                {
                    var customProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true, limitToDefaultResources: true);
                    if (customProviders.Count > 0)
                    {
                        Config.DefaultProvider = customProviders[0];
                        SettingsProvider.LastUsedSettingsProvider = Config.DefaultProvider;
                        Logger.LogMessage("Found custom settings provider under '" + AssetDatabase.GetAssetPath(Config.DefaultProvider) + "'. If you want to use different ones then please change the 'Default Provider' field.");
                    }
                    else
                    {
                        var fileName = EditorRuntimeUtils.GetProjectFileName("SettingsProvider (", ")");
                        var provider = SettingsProvider.EditorCreateProviderBasedOnTemplate(
                            SettingsGeneratorSettings.SettingsDirPath,
                            fileName, false);

                        Config.DefaultProvider = provider;
                        SettingsProvider.LastUsedSettingsProvider = Config.DefaultProvider;
                        createdNewAssets = true;

                        Logger.LogMessage("Created settings files under: " + SettingsGeneratorSettings.SettingsDirPath + fileName);
                    }
                }
                
                EditorUtility.SetDirty(Config);
                AssetDatabase.SaveAssetIfDirty(Config);
            }
            
            // show/hide containers
            _providerCtn.style.display = DisplayStyle.Flex;
            _visualsCtn.style.display = DisplayStyle.None;
            _addSettingContainer.style.display = DisplayStyle.None;
            
            // Update provider container ui
            _providerWelcomeCtn.style.display = createdNewAssets ? DisplayStyle.Flex : DisplayStyle.None;
            
            // Update UI tab
            _tabbar.Q<Button>(className: "tab-button-1").SetEnabled(Config.DefaultProvider != null);

            recreateProviderInspector();
        }

        private void recreateProviderInspector()
        {
            if (Config.DefaultProvider != null)
            {
                _providerInspectorCtn.Clear();
                _providerInspectorCtn.AddInspector(Config.DefaultProvider.SerializedObject);
            }

            if (Config.DefaultProvider != null && Config.DefaultProvider.SettingsAsset != null)
            {
                _settingsInspectorCtn.Clear();
                _settingsInspectorCtn.AddInspector(Config.DefaultProvider.SettingsAsset.SerializedObject);
            }
        }

        private VisualElement createChooseProviderGUI(VisualElement root)
        {
            _providerCtn = root.AddContainer("ChooseProvider", "grow", "content-container");
            var ctn = _providerCtn;
            
            // Welcome ctn
            _providerWelcomeCtn = ctn.AddContainer("Welcome", "shrink");
            ctn.Add(_providerWelcomeCtn);
            _providerWelcomeCtn.AddLabel("Welcome", "h1");
            _providerWelcomeCtn.AddLabel("It seems this is a new project without any settings.\n" +
                                         "We have created the necessary assets for you under Assets/Resource/SettingsGenerator/.\n" +
                                         "Hope that's okay. Please do not move them outside the Resources folder.", "word-wrap");
            _providerWelcomeCtn.AddButton("Show me the assets", (e) =>
                {
                    var customProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true, limitToDefaultResources: true);
                    if(customProviders.Count > 0)
                        EditorGUIUtility.PingObject(customProviders[0]);
                }, "end", "shrink");


            // Configs
            var configCtn = ctn.AddContainer("Configuration").AddScrollView("ScrollView", "mb-10");
            configCtn.AddLabel("If you want to add some new settings to your UI please click the 'UI' tab on the top right.", "word-wrap", "mb-10");
            
            // Providers
            configCtn.AddLabel("Provider", "h1", "mt-10");
            configCtn.AddLabel("The 'provider' is a configuration object that serves as a link to the settings. It that 'provides' the settings to whoever needs it (thus the name).", "word-wrap", "mb-5");

            var providerObjField = configCtn.AddObjectField("Default Provider:", Config.DefaultProvider, onProviderChanged, allowSceneObjects: false, "mb-10", "dont-shrink");
            providerObjField.bindingPath = SettingsGeneratorSettings._DefaultProviderFieldName;
            providerObjField.Bind(Config.SerializedObject);

            var applyButton = configCtn.AddButton("Apply to Scene", onApplyProviderToScene);
            applyButton.tooltip = "If you press this button it will update all settings related objects in the scene with the current default provider.";

            var providerFoldout = configCtn.AddFoldout("Provider Details");
            providerFoldout.style.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
            providerFoldout.BorderRadius(3f);
            providerFoldout.value = false;
            providerFoldout.AddLabel("The options below are valid for all settings within the current default provider.");
            _providerInspectorCtn = providerFoldout.AddContainer("ProviderInspectorContainer", "mb-10");
            
            // Settings
            configCtn.AddLabel("Settings", "h1", "mt-20");
            configCtn.AddLabel("Below you will find the details of each setting.", "mb-5");
            configCtn.AddLabel("PLEASE NOTICE: These are not the UI elements because each setting may have multiple UI interfaces. " +
                               "An example would be a volume slider that is shown both in the main menu and the pause menu (two interfaces but one and the same setting).", "word-wrap", "mb-5");

            var settingsFoldout = configCtn.AddFoldout("Settings Details");
            settingsFoldout.style.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
            settingsFoldout.BorderRadius(3f);
            settingsFoldout.value = false;
            _settingsInspectorCtn = settingsFoldout.AddContainer("SettingsInspectorContainer","mb-10");

            return _providerCtn;
        }

        private void onProviderChanged(ChangeEvent<Object> evt)
        {
            var target = evt.target as ObjectField;
            // Abort if demo provider is used in non-demo scenes.
            var oldProvider = (SettingsProvider)evt.previousValue;
            var newProvider = (SettingsProvider)evt.newValue;
            EditorApplication.delayCall += () =>
            {
                if (newProvider != null)
                {
                    if (!isDemoScene())
                    {
                        if (!newProvider.EditorIsExampleProvider())
                        {
                            // All good, update the default.
                            Config.DefaultProvider = newProvider;
                            SettingsProvider.LastUsedSettingsProvider = Config.DefaultProvider;
                            // Update list in "add settings" dialog.
                            if (_settingIdsListView != null && Config.DefaultProvider != null && Config.DefaultProvider.SettingsAsset != null)
                            {
                                _settingIdsListView.itemsSource = Config.DefaultProvider.SettingsAsset.GetAllSettings();
                                _settingIdsListView.Rebuild();
                            }
                        }
                        else
                        {
                            // Don't allow demo providers in regular scenes.
                            if (oldProvider != null && !oldProvider.EditorIsExampleProvider())
                                Config.DefaultProvider = oldProvider;
                            else
                                Config.DefaultProvider = null;
                
                            Logger.LogWarning(
                                $"You have chosen a provider from the examples as the default. Please create your own. Reverting to: " +
                                ((oldProvider != null) ? oldProvider.name : "Null"));
                        }   
                    }
                }

                _tabbar.Q<Button>(className: "tab-button-1").SetEnabled(Config.DefaultProvider != null);
                recreateProviderInspector();
            };
        }

        private void onApplyProviderToScene(ClickEvent evt)
        {
            //if (!isDemoScene())
            {
                SettingsProvider.EditorSetProviderInScene(Config.DefaultProvider);
            }
            //else
            //{
            //    Logger.LogMessage("This is not allowed in demo scenes to avoid breaking the demos. Try it in your own settings scene instead.");
            //}
        }

        private void bindProviderItem(Label field, int index, ListView listView)
        {
            var provider = listView.GetItemSourceAt<SettingsProvider>(index);
            if (provider != null)
            {
                field.text = provider.name;
                if (provider.EditorIsExampleProvider())
                    field.AddToClassList("warning");
                else
                    field.RemoveFromClassList("warning");
            }
        }

        private void onProviderSelectionChange(int index, ListView listView)
        {
            if (index < 0)
            {
                Config.DefaultProvider = null;
                return;
            }

            Config.DefaultProvider = listView.GetItemSourceAt<SettingsProvider>(index);
        }
    }
}
