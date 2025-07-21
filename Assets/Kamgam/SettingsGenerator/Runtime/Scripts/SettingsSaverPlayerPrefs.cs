using System;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "SettingsSaverPlayerPrefs", menuName = "SettingsGenerator/Saver/PlayerPrefs", order = 1)]
    public class SettingsSaverPlayerPrefs : SettingsSaverBase
    {
        public override void LoadInto(string key, Settings settings)
        {
            string json = PlayerPrefs.GetString(key, null);
                
            if (!string.IsNullOrEmpty(json))
            {
                SettingsSerializer.FromJson(json, settings);
            }
        }
        
        public override void Save(string key, Settings settings)
        {
            var json = SettingsSerializer.ToJson(settings);
            
            if (!string.IsNullOrEmpty(json))
            {
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
        }

        public override void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}
