using System;
using System.IO;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "SettingsSaverJson", menuName = "SettingsGenerator/Saver/Json", order = 2)]
    public class SettingsSaverJson : SettingsSaverBase
    {
        public bool LogSavePath = false;
        
        public override void LoadInto(string key, Settings settings)
        {
            var filePath = getFilePath(key);

            // Abort if not found
            if (!File.Exists(filePath))
                return;

            // Read file text
            var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

            // Apply to settings
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
                var filePath = getFilePath(key);

                // Write to tmp file first.
                File.WriteAllText(filePath + ".tmp", json, System.Text.Encoding.UTF8);

                // Check if tmp file now exists.
                // If you want to be extra sure you could do a validity check of .tmp.
                if (File.Exists(filePath + ".tmp"))
                {
                    // Delete existing
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    // Finally move the tmp file.
                    File.Move(filePath + ".tmp", filePath);

                    // Log path (for testing)
                    if (LogSavePath)
                        Debug.Log("Saved to: " + filePath);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }
            }
        }
        
        public override void Delete(string key)
        {
            var filePath = getFilePath(key);

            // Delete file
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Delete .meta file in editor
#if UNITY_EDITOR
            if (File.Exists(filePath + ".meta"))
            {
                File.Delete(filePath + ".meta");
                UnityEditor.AssetDatabase.Refresh();
            }
#endif
        }
        
        string getFilePath(string key)
        {
            return Application.dataPath + "/" + key + ".json";
        }

    }
}
