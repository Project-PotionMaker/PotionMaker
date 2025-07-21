using UnityEngine;
using Kamgam.UGUIComponentsForSettings;
using System.Text.RegularExpressions;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/InputBindingUGUIResolver")]
    [RequireComponent(typeof(InputBindingUGUI))]
    public class InputBindingUGUIResolver : SettingResolver, ISettingResolver
    {
        protected InputBindingUGUI inputBindingUGUI;
        public InputBindingUGUI InputBindingUGUI
        {
            get
            {
                if (inputBindingUGUI == null)
                {
                    inputBindingUGUI = this.GetComponent<InputBindingUGUI>();
                }
                return inputBindingUGUI;
            }
        }

#if ENABLE_INPUT_SYSTEM
        public delegate bool ResolveBindingConflictDelegate(
            string previousBindingPath, string newBindingPath,
            InputBindingConnection currentConnection, InputBindingConnection conflictingConnection);

        /// <summary>
        /// Called if the chosen binding is in conflict with the existing one.
        /// The return value defines whether the binding will continue (return true) or abort and revert (return false).
        /// </summary>
        public static ResolveBindingConflictDelegate ResolveBindingConflictFunc = null;

        /// <summary>
        /// If true then duplicate rebinding will be aborted and reverted to the previous value, unless OnBindingConflict is defined.
        /// If it is defined then the return value of that callback will determine the behaviour.
        /// </summary>
        [FormerlySerializedAs("BlockBindingConflicts")] [Tooltip("If true then duplicate rebinding will be aborted and reverted to the previous value, unless OnBindingConflict is defined.")]
        public bool BlockOnBindingConflict = false;
#endif

        [System.NonSerialized]
        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[]
        {
            SettingData.DataType.String
        };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        [Header("Debug")]
        public bool LogLocalizedBindingPath = false;

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            InputBindingUGUI.OnChanged += onChanged;
            
#if ENABLE_INPUT_SYSTEM
            // Hook up with a checker method that checks for conflicting bindings.
            InputBindingUGUI.InputBinding.CheckBindingPathFunc = checkBindingForDuplicates;
#endif

            // Hook up with the localization.
            InputBindingUGUI.PathToDisplayNameFunc = localizeKeyCode;
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                LocalizationProvider.GetLocalization().AddOnLanguageChangedListener(onLanguageChanged);
            }

            if (HasValidSettingForID(ID, GetSupportedDataTypes()) && HasActiveSettingForID(ID))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);
            }
        }

#if ENABLE_INPUT_SYSTEM
        protected bool checkBindingForDuplicates(string previousPath, string path)
        {
            if (!BlockOnBindingConflict && ResolveBindingConflictFunc == null)
                return true;

            if (BlockOnBindingConflict || ResolveBindingConflictFunc != null)
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                if (setting == null)
                    return true;

                var settingConnection = setting.GetConnectionInterface() as InputBindingConnection;
                if (settingConnection == null)
                    return true;

                var connections = InputBindingConnection.Connections;
                foreach (var connection in connections)
                {
                    // Ignore self
                    if (connection == settingConnection)
                        continue;
                    
                    if (connection.Get() == path && (ResolveBindingConflictFunc == null || !ResolveBindingConflictFunc.Invoke(previousPath, path, settingConnection, connection)))
                        return false;
                }
            }

            return true;
        }
#endif

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (InputBindingUGUI != null)
            {
                InputBindingUGUI.OnChanged -= onChanged;
                InputBindingUGUI.PathToDisplayNameFunc = null;
            }

            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                LocalizationProvider.GetLocalization().RemoveOnLanguageChangedListener(onLanguageChanged);
        }

        protected void onLanguageChanged(string language)
        {
            Refresh();
        }

        protected string localizeKeyCode(string bindingPath)
        {
#if UNITY_EDITOR
            if (LogLocalizedBindingPath)
                Logger.LogMessage("Localizing selected path: '" + bindingPath + "'", this.gameObject);
#endif

            // Only use the translation if it is available.
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                string term = bindingPath;
                if (LocalizationProvider.GetLocalization().HasTerm(term))
                {
                    return LocalizationProvider.GetLocalization().Get(term);
                }
            }
            
            return bindingPathToDisplayName(bindingPath);
        }

        protected void onChanged(string bindingPath)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetString(ID);
            setting.SetValue(bindingPath);
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetString(ID);

            if (setting == null)
                return;

            InputBindingUGUI.InputBinding.SetBindingPath(setting.GetValue());
#if ENABLE_INPUT_SYSTEM
            if (setting.GetConnection() is InputBindingConnection connection)
                InputBindingUGUI.InputBinding.AllowComposite = connection.IsComposite();
 #endif

            try
            {
                stopPropagation = true;

                if (InputBindingUGUI.PathToDisplayNameFunc == null)
                {
                    InputBindingUGUI.PathToDisplayNameFunc = localizeKeyCode;
                }

                InputBindingUGUI.UpdateDisplayName();
            }
            finally
            {
                stopPropagation = false;
            }
        }

        protected string bindingPathToDisplayName(string bindingPath)
        {
            if (bindingPath == null)
                return null;

            // This is geared toward paths like "<Keyboard>/s" => "S";
            bindingPath = Regex.Replace(bindingPath, "<[^>]*>/", "");
            if (bindingPath.Length < 6)
                bindingPath = bindingPath.ToUpper();

            return bindingPath;
        }
    }
}
