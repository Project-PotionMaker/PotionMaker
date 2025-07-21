using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using Kamgam.LocalizationForSettings;
using Kamgam.UGUIComponentsForSettings;
using UnityEngine.UI;

namespace Kamgam.SettingsGenerator
{
    // Ad a new setting view.
    
    public partial class CreateSettingUGUIWindow : EditorWindow
    {
        protected VisualElement _addSettingContainer;
        protected ListView _settingIdsListView;
        protected DropdownField _addExistingStyleDropDown;
        protected int _selectedSettingIdIndex = -1;
        protected List<StyleChoice> _addExistingSettingStyleChoices = new List<StyleChoice>();
        protected bool? _preferConsoleStyle = false;

        private VisualElement createAddSettingUI(VisualElement root)
        {
            var ctn = _addSettingContainer = root.AddContainer("Add Setting", "grow", "content-container");

            ctn.AddLabel("Add Setting", "h1");
            var providerObjField = ctn.AddObjectField<SettingsProvider>("Default Provider:", Config.DefaultProvider, onProviderChanged, allowSceneObjects: false, "dont-shrink");
            providerObjField.name = "ProviderObjField";
            providerObjField.bindingPath = SettingsGeneratorSettings._DefaultProviderFieldName;
            providerObjField.Bind(Config.SerializedObject);
            providerObjField.SetEnabled(false);
            
            var uiContainerField = ctn.AddObjectField<GameObject>("UI Container:", _uiContainer, null, allowSceneObjects: true, "mb-10", "dont-shrink");
            uiContainerField.name = "UIContainerField";
            uiContainerField.SetEnabled(false);
            uiContainerField.tooltip = "New settings will be added to this container. Update by selecting a UI element in your scene.";

            var horizontalCtn = ctn.AddContainer("Container", "horizontal", "grow");
            var addExistingCtn = horizontalCtn.AddContainer("AddExistingContainer", "w-half");
            var createNewCtn = horizontalCtn.AddContainer("CreateNewContainer", "w-half", "pl-20");

            // Add Existing Ctn
            {
                addExistingCtn.AddLabel("Add Existing", "h1", "mb-10");
                addExistingCtn.AddLabel("Here you can choose to add new settings from the existing setting IDs in the selected provider.", "word-wrap", "mb-10");
                var searchbar = addExistingCtn.AddContainer("Header", "horizontal");
                searchbar.AddTextField(onFilterSettingIds, "grow");

                _settingIdsListView = new ListView();
                _settingIdsListView.AddToClassList("ui-settings-list");
                _settingIdsListView.makeItem = () =>
                {
                    var lbl = new Label();
                    lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
                    return lbl;
                };
                _settingIdsListView.bindItem = (e, i) => bindSettingIdInList(e as Label, i, _settingIdsListView);
                _settingIdsListView.reorderable = false;
                _settingIdsListView.AddToClassList("grow");
#if UNITY_2022_1_OR_NEWER
                _settingIdsListView.selectionChanged += (e) =>
#else
                _settingIdsListView.onSelectionChange += (e) =>
#endif
                {
                    _selectedSettingIdIndex = _settingIdsListView.selectedIndex;
                    var setting = _settingIdsListView.itemsSource[_selectedSettingIdIndex] as ISetting;
                    updateAddExistingSettingStyleChoices(setting);
                };
                // Auto create on double click.
#if UNITY_2022_1_OR_NEWER
                _settingIdsListView.itemsChosen += (e) =>
#else
                _settingIdsListView.onItemsChosen += (e) =>
#endif
                {
                    _selectedSettingIdIndex = _settingIdsListView.selectedIndex;
                    var setting = _settingIdsListView.itemsSource[_selectedSettingIdIndex] as ISetting;
                    updateAddExistingSettingStyleChoices(setting);
                    onAddExisting(null);
                };
                addExistingCtn.Add(_settingIdsListView);
                
                _addExistingStyleDropDown = addExistingCtn.AddDropDown("Style", options: null, callback: onAddExistingStyleChanged, "dropdown-shrinking-label");
                updateAddExistingSettingStyleChoices(null);
                
                addExistingCtn.AddButton("Add", onAddExisting);
            }
            
            // Create New Ctn
            {
                createNewCtn.AddLabel("Create New", "h1", "mb-10");
                createNewCtn.AddLabel("To create a new setting please go to the 'Configuration' tab (top left) and use the + Button in the:\n" +
                                      " 'Settings'\n" +
                                      "   'Settings Details'\n" +
                                      "      'Bool/Int/...' section.", "word-wrap");
            }

            // Bottom menu
            var bottomMenu = ctn.AddContainer("BottomMenu", "horizontal", "dont-shrink");
            bottomMenu.AddButton("Cancel", onCancel, "grow");

            return _addSettingContainer;
        }

        private void onAddExistingStyleChanged(ChangeEvent<string> evt)
        {
            // For convenience so the last selected style will be preferred.
            _preferConsoleStyle = evt.newValue.ToLower().Contains("console");
        }

        private void onShowAddSettingDialog(ClickEvent evt)
        {
            if (Config.DefaultProvider == null)
            {
                Logger.LogError("Please choose default provider first!");
                return;
            }
            
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
            _visualsCtn.style.display = DisplayStyle.None;
            _addSettingContainer.style.display = DisplayStyle.Flex;
            
            Config.DefaultProvider.SettingsAsset.RebuildSettingsCache();
            _settingIdsListView.itemsSource = Config.DefaultProvider.SettingsAsset.GetAllSettings();
            
            var providerObjField = _addSettingContainer.Q<ObjectField>("ProviderObjField");
            providerObjField.value = Config.DefaultProvider;
            
            var uiContainerField = _addSettingContainer.Q<ObjectField>("UIContainerField");
            uiContainerField.value = _uiContainer;
            
            // Try to find a resolver in the scene and update preferConsole based on the prefab name.
            if (!_preferConsoleStyle.HasValue)
            {
                _preferConsoleStyle = false;
                
                var resolverInScene = CompatibilityUtils.FindObjectOfType<SettingResolver>(includeInactive: true);
                if (PrefabUtility.IsPartOfPrefabInstance(resolverInScene.gameObject))
                {
                    var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(resolverInScene.gameObject);
                    Debug.Log(path);
                    if (path.ToLower().Contains("console"))
                        _preferConsoleStyle = true;
                }
            }
        }

        private void onCancel(ClickEvent evt)
        {
            onShowChooseVisual(evt);
        }

        private void onAddExisting(ClickEvent evt)
        {
            if (_selectedSettingIdIndex >= 0 && _selectedSettingIdIndex < _settingIdsListView.itemsSource.Count)
            {
                var setting = _settingIdsListView.itemsSource[_selectedSettingIdIndex] as ISetting;
                createFromSetting(setting);
                
                // return to visuals overview // Not sure if that is a good idea (uses usually want to add multiple settings)
                // onShowChooseVisual(evt);
            }
        }

        private void bindSettingIdInList(Label field, int index, ListView listView)
        {
            field.text = (listView.itemsSource[index] as ISetting).GetID();
        }

        private void onFilterSettingIds(ChangeEvent<string> evt)
        {
            var searchTerm = evt.newValue.Trim().ToLower();
            var settingIds = Config.DefaultProvider.SettingsAsset.GetAllSettings()
                .Where( s =>
                {
                    if (searchTerm.IsNullOrEmpty() || s.GetID().ToLower().Contains(searchTerm))
                        return true;
                    
                    if (s.HasConnectionObject() && s.GetConnectionSO().name.ToLower().Contains(searchTerm))
                        return true;
                    
                    if (s.HasConnectionObject() && s.GetConnectionSO().GetType().Name.ToLower().Contains(searchTerm))
                        return true;

                    return false;
                })
                .ToList();
            _settingIdsListView.itemsSource = settingIds;
            _settingIdsListView.Rebuild();
        }

        void updateAddExistingSettingStyleChoices(ISetting setting)
        {
            _addExistingSettingStyleChoices.Clear();

            // Get all style choices based on prefabs.
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
                _addExistingSettingStyleChoices.Add(entry);
                
                PrefabUtility.UnloadPrefabContents(go);
            }

            // Filter by prefabs and their matching data types.
            if (setting != null)
            {
                _addExistingStyleDropDown.SetEnabled(true);
                
                var filteredChoices = _addExistingSettingStyleChoices.Where(s =>
                {
                    // Check data type match.
                    if (!s.SupportedDataTypes.Contains(setting.GetDataType()))
                        return false;
                   
                    // Check if prefabs can be converted.
                    if (!SettingsGeneratorPrefabStyleConverter.PrefabSupportsDataType(s.PrefabAssetPath, setting.GetDataType()))
                        return false;
                    
                    // TODO: Once we support UI Toolkit then add a check for it here too.

                    return true;
                }).Select(s => s.Name).ToList();

                if (filteredChoices.Count > 0)
                {
                    var choice = filteredChoices.FirstOrDefault(c =>
                    {
                        if (!_preferConsoleStyle.HasValue)
                            return true;
                        
                        bool isConsole = c.ToLower().Contains("console");
                        return _preferConsoleStyle.Value ? isConsole : !isConsole;
                    });
                    _addExistingStyleDropDown.choices = filteredChoices;
                    if (!choice.IsNullOrEmpty())
                        _addExistingStyleDropDown.value = choice;
                    else
                        _addExistingStyleDropDown.value = filteredChoices[0];
                }
                else
                {
                    _addExistingStyleDropDown.choices = new List<string>(){"Custom"};
                    _addExistingStyleDropDown.value = "Custom";
                }
            }
            else
            {
                _addExistingStyleDropDown.choices = new List<string>();
                _addExistingStyleDropDown.SetEnabled(false);
            }
        }

        void createFromSetting(ISetting setting)
        {
            var style = _addExistingSettingStyleChoices.FirstOrDefault(s => s.Name == _addExistingStyleDropDown.value);
            if (style == null)
            {
                Logger.LogError($"Could not create setting {setting.GetID()} because the style {_addExistingStyleDropDown.value} could not be found.");
                return;
            }

            // Debug.Log($"Creating {setting.GetID()} with style {style.Name}.");

            var parent = _uiContainer;
            if (parent == null || EditorUtility.IsPersistent(parent) || parent.transform as RectTransform == null)
            {
                EditorUtility.DisplayDialog(
                    "No UI container selected!",
                    "Please select the ui container you wish to add the setting to.",
                    "Ok"
                );
                return;
            }
            
            // Check if parent is a canvas or if  it does not have a layout group. If it doesn't ask whether or not to create one.
            bool hasLayout = parent.GetComponent<HorizontalLayoutGroup>() != null ||
                             parent.GetComponent<VerticalLayoutGroup>() != null ||
                             parent.GetComponent<GridLayoutGroup>() != null;
            var childResolver = parent.GetComponentInChildren<SettingResolver>(includeInactive: true);
            if ((childResolver == null || childResolver.transform.parent != parent.transform) && !hasLayout)
            {
                bool createContainer = EditorUtility.DisplayDialog(
                    "Do you want to create a settings container?",
                    "It seems like you are creating a new setting in an empty element.\n\n" +
                    "Would you like to create a settings container with a vertical layout group?",
                    "Yes (Recommended)", "No"
                );
                if (createContainer)
                {
                    var container = new GameObject("Settings", typeof(RectTransform));
                    container.transform.SetParent(parent.transform);
                    var rect = container.transform as RectTransform;
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = Vector2.zero;
                    var layout = container.AddComponent<VerticalLayoutGroup>();
                    layout.padding = new RectOffset(15, 15, 15, 15);
                    layout.childForceExpandHeight = false;
                    parent = container;
                }
            }
            
            var provider = Config.DefaultProvider;
            if (provider == null || provider.SettingsAsset == null || !provider.SettingsAsset.HasID(setting.GetID()) || parent == null)
                return;

            var prefab = style.GetPrefab();
            if (prefab == null)
            {
                EditorUtility.DisplayDialog(
                    "Missing Style Prefab",
                    $"Aborted because the style prefab '{style.PrefabAssetPath}' could not be loaded.",
                    "Ok"
                );
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
            instance.name = instance.name.Replace("(Setting)", "(" + setting.GetID() + ")");
            var resolver = instance.GetComponentInChildren<SettingResolver>();
            Undo.RegisterCreatedObjectUndo(instance, "Added new setting UI.");
            // Navigation
            updateNavigationOfUGUIInstance(instance);

            if (resolver != null)
            {
                // Link to provider
                resolver.SettingsProvider = provider;
                resolver.ID = setting.GetID();
                EditorUtility.SetDirty(resolver);
                
                // Apply default UI parameters
                SettingsGeneratorUIParameters.ApplyUIParametersToUGUI(resolver);
                
                // Update label
                var label = instance.transform.GetComponentsInChildren<TMPro.TextMeshProUGUI>(includeInactive: true).FirstOrDefault(tf => tf.name.ToLower() == "label");
                if (label != null)
                {
                    // Find the "Label" textfield and set the name based on the localization.
                    if (resolver.LocalizationProvider != null)
                    {
                        // First try to get the translation term from the explicit list.
                        // If that fails then get it based on text distance.
                        if (!SettingsGeneratorPrefabStyleConverter.SettingIdToLabel.TryGetValue(resolver.ID, out var term))
                        {
                            term = getClosestLocalizationTerm(resolver.LocalizationProvider, resolver.ID, 0.2);    
                        }
                        
                        var translator = label.GetComponent<LocalizeTMPro>();
                        translator.Term = term;
                        translator.Localize();
                    }
                }

                // Activate setting
                if (!setting.IsActive)
                {
                    setting.IsActive = true;
                    EditorUtility.SetDirty(provider.SettingsAsset);
                    SaveAssetHelper.SaveAssetIfDirty(provider.SettingsAsset);
                }
                
                EditorUtility.SetDirty(parent);
                EditorGUIUtility.PingObject(instance);
            }
        }

        private static void updateNavigationOfUGUIInstance(GameObject instance)
        {
            var parent = instance.transform.parent;
            
            // update navigation (find previous and next sibling)
            int instanceIndex = instance.transform.GetSiblingIndex();
            var instanceSelectable = instance.transform.GetComponentInChildren<Selectable>();
            var instanceNavigation = instance.transform.GetComponentInChildren<AutoNavigationOverrides>();

            if (instanceSelectable == null)
                return;
            
            // Search for previous and next selectable and update auto navigation if necessary.
            Selectable previousSelectable = null;
            AutoNavigationOverrides previousNavigation = null;
            for (int i = instanceIndex - 1; i >= 0; i--)
            {
                var selectable = parent.transform.GetChild(i).GetComponentInChildren<Selectable>();
                if (selectable != null)
                {
                    previousSelectable = selectable;
                    previousNavigation = parent.transform.GetChild(i).GetComponentInChildren<AutoNavigationOverrides>();
                    break;
                }
            }

            Selectable nextSelectable = null;
            AutoNavigationOverrides nextNavigation = null;
            for (int i = instanceIndex + 1; i < parent.transform.childCount; i++)
            {
                var selectable = parent.transform.GetChild(i).GetComponentInChildren<Selectable>();
                if (selectable != null)
                {
                    nextSelectable = selectable;
                    nextNavigation = parent.transform.GetChild(i).GetComponentInChildren<AutoNavigationOverrides>();
                    break;
                }
            }

            // Apply the new navigation if necessary
            if (instanceNavigation == null)
                return;

            var upTarget = instanceNavigation.FindSelectableOnUp();
            if (upTarget != previousSelectable)
            {
                instanceNavigation.SelectOnUpOverride = previousSelectable;
                EditorUtility.SetDirty(instanceNavigation);
                PrefabUtility.RecordPrefabInstancePropertyModifications(instanceNavigation);
            }

            var downTarget = instanceNavigation.FindSelectableOnDown();
            if (downTarget != nextSelectable)
            {
                instanceNavigation.SelectOnDownOverride = nextSelectable;
                EditorUtility.SetDirty(instanceNavigation);
                PrefabUtility.RecordPrefabInstancePropertyModifications(instanceNavigation);
            }
        }

        string getClosestLocalizationTerm(LocalizationProvider localizationProvider, string searchTerm, double maxDistance)
        {
            var loc = localizationProvider.GetLocalization() as Localization;
            
            if (loc == null)
                return searchTerm;
            
            string closestTerm = null;
            double distance = float.MaxValue;
            for (int i = 0; i < loc.GetTranslationCount(); i++)
            {
                var term = loc.GetTranslationAt(i).GetTerm();
                var d = JaroWinklerDistance.Distance(term.ToLower(), searchTerm.ToLower());
                if (d < distance )
                {
                    distance = d;
                    closestTerm = term;
                }
            }

            if (distance < maxDistance)
                return closestTerm;
            else
                return searchTerm;
        }
    }
}