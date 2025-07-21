# if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace Kamgam.SettingsGenerator
{
    public static class EditorRuntimeUtils
    {
        public static SettingsProvider LastOpenedProvider;
        public static SettingResolver LastOpenedResolverWithProvider;
        public static Settings LastOpenedSettingsAsset;

        public static SettingsProvider FindPreferredSettingsProvider()
        {
            // First try using the last used provider
            if (SettingsGeneratorSettings.GetOrCreate().DefaultProvider != null)
            {
                return SettingsGeneratorSettings.GetOrCreate().DefaultProvider;
            }
            else if (LastOpenedProvider != null)
            {
                return LastOpenedProvider;
            }
            // Second try the provider from the last used UI.
            else if (LastOpenedResolverWithProvider != null && LastOpenedResolverWithProvider.SettingsProvider != null)
            {
                return LastOpenedResolverWithProvider.SettingsProvider;
            }
            // Third: search in assets and used the first found
            // Right now the shallower the path of a settings file is the soon it will be returned.
            // Which is okay too since user settings are usually in shallower paths than the default
            // examples or templates.
            // TODO/N2H: sort by modification time and the select the newest.
            else
            {
                var resourcesProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true, limitToDefaultResources: true);
                if (resourcesProviders.Count > 0)
                    return resourcesProviders[0];
                
                resourcesProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: true, limitToDefaultResources: false);
                if (resourcesProviders.Count > 0)
                    return resourcesProviders[0];
                
                resourcesProviders = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: false, limitToDefaultResources: false);
                if (resourcesProviders.Count > 0)
                    return resourcesProviders[0];
            }

            return null;
        }

        /// <summary>
        /// Returns the path without assets und without and leading or trailing / and all \ are converted to /.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string makePathRelativeToAssets(string path)
        {
            path = path.Replace("\\", "/");
            path = path.Trim('/');
            path = path.Replace("Assets/", "");
            return path;
        }
        
        public static void CreateFolder(string path, bool log = true)
        {
            path = path.Replace("\\", "/");
            path = path.Trim('/');
            path = path.Replace("Assets/", "");
            var folders = path.Split('/');

            string currentPath = "Assets";

            bool created = false;
            foreach (string folder in folders)
            {
                currentPath = System.IO.Path.Combine(currentPath, folder);
                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    string parentFolder = System.IO.Path.GetDirectoryName(currentPath);
                    AssetDatabase.CreateFolder(parentFolder, folder);
                    created = true;
                }
            }
            
            if (created && log)
                Debug.Log($"Created folder: '{path}'. Hope that's okay. Please don't delete or rename it.");
        }

        public static string GetProjectFileName(string prefix = "", string suffix = "")
        {
            // Use product name first
            var name = PlayerSettings.productName;
            name = Regex.Replace(name, @"[^-A-Za-z0-9_. ]", "");

            // Then project folder name
            if (string.IsNullOrEmpty(name))
            {
                string[] s = Application.dataPath.Split('/');
                name = s[s.Length - 2];
                name = Regex.Replace(name, @"[^-A-Za-z0-9_. ]", "");
            }

            // Then random name
            if (string.IsNullOrEmpty(name))
            {
                name = Random.Range(0, 9999).ToString();
            }

            return prefix + name + suffix;
            
        }
    }
}
#endif
