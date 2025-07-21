using Kamgam.LocalizationForSettings;
using UnityEngine;
#if TextMeshPro
using System.Collections.Generic;
using TMPro;
#endif

namespace Kamgam.SettingsGenerator.Examples
{
    public class UnappliedSettingsDialog : MonoBehaviour
    {
        public LocalizationProvider Localization;
        
#if TextMeshPro
        public TextMeshProUGUI SettingsList;
        
        public void SetSettings(List<ISetting> settings)
        {
            SettingsList.text = "";
            
            for (int i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
                SettingsList.text += " * " + Localization.Get(setting.GetID(), setting.GetID());
            }
        }
#endif
    }
}
