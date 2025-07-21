#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using Kamgam.UGUIComponentsForSettings;
using UnityEngine;

using UnityEngine.InputSystem;

namespace Kamgam.SettingsGenerator
{
    public class InputBindingConnection : Connection<string>
    {
        /// <summary>
        /// A static list of all created connections.
        /// </summary>
        public static List<InputBindingConnection> Connections = new List<InputBindingConnection>();

        // Reset static variables on play mode enter to support disabling domain reload.
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlayModeEnter()
        {
            Connections.Clear();
        }
#endif
        

        public static bool LogErrorOnBindingFail = true;

        /// <summary>
        /// The input action asset. An override will be added to the binding in this asset if Set(KeyCombination keyCombination) is called.
        /// </summary>
        protected InputActionAsset _inputActionAsset;

        /// <summary>
        /// A string uniquely identifying the binding in the InputSystem (GUID).
        /// </summary>
        protected string _bindingId;

        public InputBindingConnection()
        {
            Connections.Add(this);
        }

        public void SetBindingId(string id)
        {
            _bindingId = id;
        }

        public string GetBindingId()
        {
            return _bindingId;
        }

        public void SetInputActionAsset(InputActionAsset asset)
        {
            _inputActionAsset = asset;
        }

        public InputActionAsset GetInputActionAsset()
        {
            return _inputActionAsset;
        }

        public void ClearOverride()
        {
            if (_inputActionAsset == null)
            {
                return;
            }

            _inputActionAsset.ClearOverride(_bindingId);
        }

        public override string Get()
        {
            return getBindingPath(getDefault: false);
        }

        public override string GetDefault()
        {
            return getBindingPath(getDefault: true);
        }

        protected string getBindingPath(bool getDefault)
        {
            if (_inputActionAsset == null)
            {
                logNoInputAssetError();
                return null;
            }

            InputBinding binding;
            bool found = _inputActionAsset.FindBinding(_bindingId, out binding);
            if (found)
            {
                if (binding.isComposite)
                {
                    return getPathsFromComposite(binding, getDefault);
                }

                // This is where getting the default differs.
                return getDefault ? binding.path : binding.effectivePath;
            }
            else
            {
                logNoBindingError();
                return null;
            }
        }

        public bool IsComposite()
        {
            if (_inputActionAsset == null)
            {
                logNoInputAssetError();
                return false;
            }

            InputBinding binding;
            bool found = _inputActionAsset.FindBinding(_bindingId, out binding);
            if (found)
            {
                return binding.isComposite;
            }
            else
            {
                logNoBindingError();
                return false;
            }
        }

        /// <summary>
        /// Goes through the composite children of the composite binding and extracts each path. It uses
        /// InputBindingForInputSystem.CompositeControlSeparator to glue the paths together into one string.
        /// <br />
        /// Returns something like "<Keyboard>/shift+<Keyboard>/space".
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="getDefault"></param>
        /// <returns></returns>
        protected string getPathsFromComposite(InputBinding binding, bool getDefault)
        {
            string paths = "";

            string bindingId = binding.id.ToString();
            var action = _inputActionAsset.GetActionOfBinding(bindingId);

            foreach (var childBinding in action.bindings)
            {
                // Notice: It is important that skip logic is the same in getPathsFromComposite and setPathsOnComposite.
                if (!childBinding.isPartOfComposite)
                    continue;

                // This is where getting the default differs.
                var path = getDefault ? childBinding.path : childBinding.effectivePath;
                if (string.IsNullOrEmpty(paths))
                {
                    paths = path;
                }
                else
                {
                    // Ignore any keys
                    if (!path.Contains("anyKey"))
                    {
                        paths += InputBindingForInputSystem.CompositeControlSeparator + path;
                    }
                }
            }

            return paths;
        }

        private static void logNoInputAssetError()
        {
            if (LogErrorOnBindingFail)
            {
                Debug.LogError("The InputActionAsset is NULL.");
            }
        }

        private void logNoBindingError()
        {
            if (LogErrorOnBindingFail)
            {
                Debug.LogError($"No binding for ID '{_bindingId}' found.");
            }
        }

        public override void Set(string overridePath)
        {
            if (_inputActionAsset == null)
            {
                if (LogErrorOnBindingFail)
                {
                    Debug.LogError("The InputActionAsset is NULL.");
                }

                return;
            }

            InputBinding binding;
            bool found = _inputActionAsset.FindBinding(_bindingId, out binding);
            if (found)
            {
                if (binding.isComposite)
                {
                    setPathsOnComposite(binding, overridePath);
                }
                else
                {
                    // If a path with the separator is used but the binding is no composite then
                    int indexOfSeparator = overridePath.IndexOf(InputBindingForInputSystem.CompositeControlSeparator);
                    if (indexOfSeparator >= 0)
                    {
                        var newPath = overridePath.Substring(0, indexOfSeparator);
    #if UNITY_EDITOR
                        Logger.Log("Binding path '" + overridePath +
                                   "'contains separator char but is not composite. Will use '" + newPath + "' instead.");
    #endif
                        overridePath = newPath;
                    }

                    var result = _inputActionAsset.ApplyBindingOverrideWithResult(_bindingId, overridePath);
                    if (!result && LogErrorOnBindingFail)
                    {
                        Debug.LogError($"No binding for ID '{_bindingId}' found.");
                    }
                }
            }

            NotifyListenersIfChanged(overridePath);
        }

        /// <summary>
        /// Goes through the composite children of the composite binding and overrides each path. This assumes
        /// that compositePath are multiple paths separated by InputBindingForInputSystem.CompositeControlSeparator.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="compositePath"></param>
        protected void setPathsOnComposite(InputBinding binding, string compositePath)
        {
            string[] paths = compositePath.Split(InputBindingForInputSystem.CompositeControlSeparator);

            string bindingId = binding.id.ToString();
            var action = _inputActionAsset.GetActionOfBinding(bindingId);

            int pathIndex = 0;
            foreach (var childBinding in action.bindings)
            {
                // Notice: It is important that skip logic is the same in getPathsFromComposite and setPathsOnComposite.
                if (!childBinding.isPartOfComposite)
                    continue;

                string childOverridePath;
                string childBindingId = childBinding.id.ToString();

                // This is where getting the default differs.
                if (paths.Length > pathIndex)
                {
                    childOverridePath = paths[pathIndex];

                    if (!string.IsNullOrEmpty(childOverridePath))
                    {
                        var result = _inputActionAsset.ApplyBindingOverrideWithResult(childBindingId, childOverridePath);
                        if (!result && LogErrorOnBindingFail)
                        {
                            Debug.LogError($"No binding for ID '{bindingId}' found.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Empty path for binding ID '{bindingId}'. Skipping.");
                    }
                }
                else
                {
    #if UNITY_EDITOR
                    Debug.Log(
                        $"Not enough paths provided for all composites. Using <Keyboard>/anyKey as fallback for extras.");
    #endif
                    // To support composite inputs we fill the excess bindings (i.e. if only one key is used in a composite)
                    // with an equivalent of <Keyboard>/anyKey (effectively making the composite into a single input).
                    // TODO: Expand this for other input types (currently only keyboard is supported).
                    childOverridePath = "<Keyboard>/anyKey";
                    var result = _inputActionAsset.ApplyBindingOverrideWithResult(childBindingId, childOverridePath);
                    if (!result && LogErrorOnBindingFail)
                    {
                        Debug.LogError($"No binding for ID '{bindingId}' found.");
                    }
                }

                pathIndex++;
            }
        }
    }
}

#endif
