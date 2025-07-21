#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{

    [CustomEditor(typeof(SettingReceiverGenericConnector))]
    public class SettingReceiverGenericConnectorEditor : Editor
    {
        private SettingReceiverGenericConnector _connector;
        
        private VisualElement _root;
        
        private PropertyField _settingIdPropertyField;
        private ListView _settingListView;
       
        private PropertyField _pathPropertyField;
        private VisualElement _pathCtn;
        private ListView _pathListView;

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            _connector = (SettingReceiverGenericConnector)target;

            if (_connector.SettingsProvider == null)
                _connector.SettingsProvider = EditorRuntimeUtils.FindPreferredSettingsProvider();

            InspectorElement.FillDefaultInspector(_root, serializedObject, this);
#if !UNITY_6000_0_OR_NEWER
            UIToolkitHeaderAttributeFixer.AddHeaderLabels(_root, serializedObject);
#endif
            
            _settingIdPropertyField = _root.Q<PropertyField>(name: "PropertyField:SettingId");
            _pathPropertyField = _root.Q<PropertyField>(name: "PropertyField:Path");
            
            // Setting selection
            var settingCtn = new VisualElement();
            settingCtn.style.marginLeft = 10;
            settingCtn.style.flexDirection = FlexDirection.Row;
            {
                // Setting ids of the provider and based on the input.
                _settingListView = new ListView();
                _settingListView.makeItem = () => new Label();
                _settingListView.bindItem = (element, index) =>
                {
                    var label = (Label)element;
                    label.text = _settingListView.itemsSource[index].ToString();
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    label.style.marginLeft = 4;
                };
                _settingListView.selectionType = SelectionType.Single;
#if UNITY_2022_3_OR_NEWER
                _settingListView.selectionChanged += OnSettingSelectionChanged;
#else
                _settingListView.onSelectionChange += OnSettingSelectionChanged;
#endif
                _settingListView.style.maxHeight = 250;
                _settingListView.style.flexGrow = 1f;
                _settingListView.style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
                _settingListView.style.marginBottom = 4;
                settingCtn.Add(_settingListView);

                var settingIndex = _settingIdPropertyField.parent.IndexOf(_settingIdPropertyField);
                _settingIdPropertyField.parent.Insert(settingIndex + 1, settingCtn);
                _settingIdPropertyField.RegisterValueChangeCallback(onSettingIdChanged);
            }

            // Property Path selection
            _pathCtn = createPathSelectionCtn(_pathPropertyField, ref _pathListView, () => _connector.Path, (s) => _connector.Path = s, OnPropertyPathSelectionChanged);
            
            updateInspector();
            waitForPropertyUIToGenerate(onBindingComplete);
            
            return _root;
        }

        private VisualElement createPathSelectionCtn(PropertyField propertyField, ref ListView listView, System.Func<string> getValueFunc, System.Action<string> setValueFunc, System.Action<IEnumerable<object>> onSelectionChanged)
        {
            var ctn = new VisualElement();
            ctn.style.marginLeft = 10;
            ctn.style.flexDirection = FlexDirection.Row;
            {
                // Back button
                var navigationRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                var backButton = new Button( () => pathBack(getValueFunc, setValueFunc)) { text = "<" };
                navigationRow.Add(backButton);
                ctn.Add(navigationRow);

                // Paths list
                listView = new ListView();
                listView.makeItem = () => new Label();
                var list = listView;
                listView.bindItem = (element, index) =>
                {
                    var label = (Label)element;
                    label.text = list.itemsSource[index].ToString();
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    label.style.marginLeft = 4;
                };
                listView.selectionType = SelectionType.Single;
#if UNITY_2022_3_OR_NEWER
                listView.selectionChanged += onSelectionChanged;
#else
                listView.onSelectionChange += onSelectionChanged;
#endif
                listView.style.maxHeight = 250;
                listView.style.flexGrow = 1f;
                listView.style.marginBottom = 4;
                ctn.style.backgroundColor = new Color(0, 0, 0, 0.3f);
                ctn.Add(listView);

                var pathIndex = propertyField.parent.IndexOf(propertyField);
                propertyField.parent.Insert(pathIndex + 1, ctn);
                propertyField.RegisterValueChangeCallback(onPathChanged);
            }

            return ctn;
        }

        private bool _bindingCompleted;
        
        private void waitForPropertyUIToGenerate(System.Action onBindingDone)
        {
            _bindingCompleted = false;
            
            _root.schedule.Execute(() => {}).Until(() =>
            {
                var propFields = _root.Query<PropertyField>().Build();
                bool haveChildren = true;
                foreach (var field in propFields)
                {
                    if (field.childCount == 0)
                    {
                        haveChildren = false;
                        break;
                    }
                }
            
                if (!haveChildren)
                {
                    // Keep waiting for the "binding" to generate the inner UI of property fields.
                    return false; 
                }
                
                // call complete
                try
                {
                    _bindingCompleted = true;
                    onBindingDone?.Invoke();
                }
                catch (System.Exception e)
                {
                    // The try/catch guards us against errors in onBindingDone which
                    // would lead to an infinite execution loops because the UNTIL predicate
                    // would never be fulfilled.
                    Debug.LogError(e.Message); 
                }
                return true;
            });
        }

        private void onBindingComplete()
        {
            onSettingIdChanged(null);
            onPathChanged(null);
        }

        private void onPathChanged(SerializedPropertyChangeEvent evt)
        {
            if (!_bindingCompleted)
                return;
            
            updateConnectionPathLists();

            var setting = _connector.SettingsProvider.SettingsAsset.GetSetting(_connector.SettingId);
            
            // If no setting was chosen then stay neutral as we can not yet determine if the field is compatible.
            if (setting == null)
            {
                _pathPropertyField.Q(className: "unity-base-text-field__input").style.backgroundColor = StyleKeyword.Null;
                return;
            }
            
            // Otherwise check if the field/property is compatible with setting datatype and colorize.
            bool isCompatible = _connector.IsSettingCompatibleWithPath();
            if (isCompatible)
            {
                if (setting.IsActive)
                    _pathPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = new Color(0f,1f,0f,0.1f);
                else
                    _pathPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = new Color(1f,1f,0f,0.1f);
            }
            else
            {
                if (string.IsNullOrEmpty(_connector.SettingId.Trim()) || string.IsNullOrEmpty(_connector.Path.Trim()))
                    _pathPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = StyleKeyword.Null;
                else
                    _pathPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = new Color(1f,0f,0f,0.1f);
            }
        }

        private void onSettingIdChanged(SerializedPropertyChangeEvent evt)
        {
            if (!_bindingCompleted)
                return;
            
            updateSettingsList();
            
            var hasProvider = _connector.SettingsProvider != null;
            var hasSettingsAsset = _connector.SettingsProvider.SettingsAsset != null;
            var hasSettingIdWithTerm = string.IsNullOrEmpty(_connector.SettingId) || _connector.SettingsProvider.SettingsAsset.GetAllSettings()
                .Select(s => s.GetID().ToLower().Trim())
                .Contains(_connector.SettingId.Trim().ToLower()); 
            if (hasProvider && hasSettingsAsset && hasSettingIdWithTerm && !string.IsNullOrEmpty(_connector.SettingId))
            {
                var setting = _connector.SettingsProvider.SettingsAsset.GetSetting(_connector.SettingId.Trim());
                if (setting.IsActive)
                    _settingIdPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = new Color(0f,1f,0f,0.1f);
                else
                    _settingIdPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = new Color(1f,1f,0f,0.1f);
            }
            else
            {
                if (string.IsNullOrEmpty(_connector.SettingId))
                    _settingIdPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = StyleKeyword.Null;
                else
                    _settingIdPropertyField.Q(className:"unity-base-text-field__input").style.backgroundColor = new Color(1f,0f,0f,0.1f);
            }
        }

        private void pathBack(System.Func<string> getFunc, System.Action<string> setFunc)
        {
            if (string.IsNullOrEmpty(getFunc()))
                return;
            
            int index = getFunc().LastIndexOf(".");
            if (index >= 0)
                setFunc(getFunc().Substring(0, index));
            else
                setFunc("");

            updateInspector();
        }

        private void updateInspector()
        {
            updateSettingsList();
            updateConnectionPathLists();
        }
        
        private void updateSettingsList()
        {
            _settingListView.itemsSource = null;
            if (_connector.SettingsProvider != null && _connector.SettingsProvider.SettingsAsset != null)
            {
                var settingIds = _connector.SettingsProvider.SettingsAsset.GetAllSettings()
                    .Where(s =>
                        string.IsNullOrEmpty(_connector.SettingId) ||
                        string.IsNullOrEmpty(_connector.SettingId.Trim()) ||
                        s.GetID().ToLower().Contains(_connector.SettingId.Trim().ToLower()) ||
                        s.HasConnectionObject() && s.GetConnectionSO().GetType().Name.ToLower().Contains(_connector.SettingId.Trim().ToLower()))
                    .Select(s => (s.IsActive ? "[X] " : "[ ] ") + s.GetID())
                    .ToList();

                settingIds.Sort();
                
                _settingListView.itemsSource = settingIds;
            }

            _settingListView.selectedIndex = -1;
            _settingListView.RefreshItems();
        }

        private void OnSettingSelectionChanged(IEnumerable<object> selectedItems)
        {
            var selectedItem = selectedItems.FirstOrDefault();
            if (selectedItem != null)
            {
                if (_connector.SettingId != selectedItem.ToString())
                {
                    _connector.SettingId =  selectedItem.ToString().Replace("[X] ","").Replace("[ ] ","").Trim();
                }
            }
        }
        
        private void updateConnectionPathLists()
        {
            var setting = _connector.Setting;
            var compatibleTypes = setting == null ? null : SettingData.CompatibleTypes[setting.GetDataType()];

            var propertyPaths = _connector.Inspector.GetPaths(_connector.Path, includeMethods: true, getOrSetMethods: false, compatibleTypes);
            propertyPaths.Sort();
            _pathListView.itemsSource = propertyPaths;
            _pathListView.selectedIndex = -1;
            _pathListView.RefreshItems();
        }

        private void OnPropertyPathSelectionChanged(IEnumerable<object> selectedItems)
        {
            var selectedItem = selectedItems.FirstOrDefault();
            if (selectedItem != null)
            {
                _connector.Path = selectedItem.ToString();
                updateInspector();
            }
        }
    }
}
#endif