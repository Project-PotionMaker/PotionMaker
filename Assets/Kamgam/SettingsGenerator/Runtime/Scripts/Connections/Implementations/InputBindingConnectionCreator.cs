#if ENABLE_INPUT_SYSTEM && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem.Composites;

namespace Kamgam.SettingsGenerator
{
    public static class InputBindingConnectionCreator
    {
        [MenuItem("Tools/Settings Generator/Create InputBindingConnections", priority = 1)]
        [MenuItem("Assets/Settings Generator/Create InputBindingConnections", priority = 10000)]
        public static void CreateBindingConnectionAssets()
        {
            var asset = Selection.activeObject as InputActionAsset;
            if (asset == null)
            {
                EditorUtility.DisplayDialog("No InputActionAsset selected.", "Please select the InputActionAsset you want to create the connections from.", "OK");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(asset);
            foreach (var binding in asset.bindings)
            {
                // Composite bindings are not supported. See manual page ~87 (we can not use ApplyBindingOverrideWithResult() on compositions).
                if (!IsBindingSupported(binding))
                    continue;

                var map = asset.GetActionMapOfBinding(binding.id.ToString());

                string fullBindingPath = map.name + "." + binding.action + "." + (binding.isComposite ? binding.name : binding.path);
                fullBindingPath = Regex.Replace(fullBindingPath, "[<>]+", "");
                fullBindingPath = Regex.Replace(fullBindingPath, "[\\\\/]+", ".");
                var fileName = string.Format("InputBindingConnection ({0}).asset", fullBindingPath);

                var filePath = System.IO.Path.GetDirectoryName(assetPath) + "/" + fileName;
                // Update if it already exists
                var connectionSO = AssetDatabase.LoadAssetAtPath<InputBindingConnectionSO>(filePath);
                if (connectionSO == null)
                {
                    connectionSO = ScriptableObject.CreateInstance<InputBindingConnectionSO>();
                    connectionSO.InputActionAsset = asset;
                    connectionSO.BindingId = binding.id.ToString();
                    AssetDatabase.CreateAsset(connectionSO, filePath);
                }
                connectionSO.InputActionAsset = asset;
                connectionSO.BindingId = binding.id.ToString();
                EditorUtility.SetDirty(connectionSO);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
        }

        public static bool IsBindingSupported(InputBinding binding)
        {
            if (!binding.isComposite)
                return true;
            
            var compositeType = InputSystem.TryGetBindingComposite( binding.effectivePath );
            // Only "OneModifier" composites are supported.
            if (compositeType != typeof(OneModifierComposite))
            {
                return false;
            }

            return true;
        }

        [MenuItem("Assets/Settings Generator/Create InputBindingConnections", true)]
        static bool ValidateCreateBindingConnectionAssets()
        {
            return Selection.activeObject is InputActionAsset;
        }
    }
}

#endif
