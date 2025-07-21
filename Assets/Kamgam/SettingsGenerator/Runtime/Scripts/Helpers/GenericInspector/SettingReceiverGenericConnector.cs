using System;
using Kamgam.UGUIComponentsForSettings;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    public class SettingReceiverGenericConnector : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("If enabled then the setting value will be pushed to the property/field/method at start.")]
        public bool ApplyOnStart = true;
        
        [Header("Setting")]
        [Tooltip("The settings provider used to find the setting by id.")]
        public SettingsProvider SettingsProvider;
        
        [Tooltip("Enter a setting id (HINT: Choose from the list below). If the input field turns green then you have entered a valid id.")]
        public string SettingId;
        
        [FormerlySerializedAs("PropertyPath")]
        [Header("Receiving Property")]
        
       [Tooltip("Enter a path to a property that matches the setting type.\n" +
                 "Use the selector below. If the field turns green then it is compatible with the setting you have chosen.")]
        public string Path;

        protected GameObjectInspector _inspector;
        public GameObjectInspector Inspector
        {
            get
            {
                if (_inspector == null)
                    _inspector = new GameObjectInspector(this.gameObject);

                return _inspector;
            }
        }

        public ISetting Setting
        {
            get
            {
                if (SettingsProvider == null || string.IsNullOrEmpty(SettingId))
                    return null;

                var settings = SettingsProvider.GetSettingsAssetOrRuntimeCopy();
                if (settings == null)
                    return null;
                
                return settings.GetSetting(SettingId);
            }
        }

        public bool IsSettingCompatibleWithPath()
        {
            if (SettingsProvider == null)
                return true;

            return Inspector.IsSettingCompatibleWithPath(SettingsProvider, SettingId, Path, !Path.IsNullOrEmpty());
        }

        public void Start()
        {
            if (SettingsProvider == null || !SettingsProvider.HasSettings() || !SettingsProvider.Settings.HasID(SettingId))
                return;

            var setting = SettingsProvider.Settings.GetSetting(SettingId);
            if (!setting.IsActive)
            {
                Logger.LogWarning($"Trying to access inactive setting '{setting.GetID()}'.");
                return;
            }

            setting.OnSettingChanged += OnSettingChanged;
            
            if (ApplyOnStart)
                setting.OnChanged(); // Trigger change in on enable.
        }

        public void OnDisable()
        {
            if (SettingsProvider == null || !SettingsProvider.HasSettings() || SettingsProvider.Settings.HasID(SettingId))
                return;
            
            var setting = SettingsProvider.Settings.GetSetting(SettingId);
            setting.OnSettingChanged -= OnSettingChanged;
        }

        private void OnSettingChanged(ISetting setting)
        {
            if (!IsSettingCompatibleWithPath())
                return;
            
            Inspector.Set<object>(Path, setting.GetValueAsObject());   
        }
    }
}