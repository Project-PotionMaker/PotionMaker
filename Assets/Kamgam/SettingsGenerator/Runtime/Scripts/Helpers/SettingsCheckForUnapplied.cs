using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Helper to auto-detect unapplied settings in OnDisable(). Handy for showing confirmation dialogs.<br />
    /// It can also be triggerd by the provider once the UI was closed (all resolvers have been set inactive).
    /// </summary>
    public class SettingsCheckForUnapplied : MonoBehaviour
    {
        [System.NonSerialized]
        private static List<SettingsCheckForUnapplied> _registry = new List<SettingsCheckForUnapplied>();
        
        // Reset static variables on play mode enter to support disabling domain reload.
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlayModeEnter()
        {
            _registry.Clear();
        }
#endif

        public static void TriggerCheck()
        {
            foreach (var checker in _registry)
            {
                if (checker != null && checker.Provider != null && checker.Provider.HasSettings())
                {
                    checker.Check();
                    
                    // Calling check on one should suffice.
                    break;
                }
            }
        }
        
        [System.NonSerialized]
        private static int _lastCheckFrame = 0;
        
        [Tooltip("Turns of the check. If disabled then this component will do nothing.")]
        [FormerlySerializedAs("Enabled")]
        public bool CheckOnDisable = true;
        
        /// <summary>
        /// Don't forget to hook this up with the right provider.
        /// </summary>
        [Tooltip("Don't forget to hook this up with the right provider.")]
        public SettingsProvider Provider;

        // public delegate void OnUnappliedSettingsDetectedDelegate(List<ISetting> unappliedSettings);
        public UnityEvent<List<ISetting>> OnUnappliedSettingsDetected;

        /// <summary>
        /// Useful for showing modal confirm dialogs after settings UI has been disabled.
        /// </summary>
        [Tooltip("Useful for showing modal confirm dialogs after settings UI has been disabled.")]
        public List<GameObject> ObjectsToShowOnUnapplied;

        [System.NonSerialized]
        public List<ISetting> _unappliedSettings = new List<ISetting>(10);

        public void OnEnable()
        {
            if (!_registry.Contains(this))
                _registry.Add(this);
        }

        public void OnDisable()
        {
            if (_registry.Contains(this))
                _registry.Remove(this);
            
            if (CheckOnDisable)
                Check();
        }
        
        public void Check()
        {
            if (Provider == null || !Provider.HasSettings())
                return;
            
            // Do not check quickly in a row.
            // Used to avoid double checks from OnDisable and the Provider UnappliedBehaviourOnClose setting. 
            if (Time.frameCount - _lastCheckFrame < 2) // Anything within 2 frames is ignored (kinda arbitrary, could be 1 too).
                return;
            
            _lastCheckFrame = Time.frameCount;

            Provider.Settings.GetUnappliedSettings(_unappliedSettings);
            if(_unappliedSettings.Count > 0)
            {
                OnUnappliedSettingsDetected?.Invoke(_unappliedSettings);

                if (ObjectsToShowOnUnapplied != null && ObjectsToShowOnUnapplied.Count > 0)
                {
                    foreach (var gameObject in ObjectsToShowOnUnapplied)
                    {
                        if (gameObject != null)
                            gameObject.SetActive(true);
                    }
                }
            }
        }

        public void LogSettings(List<ISetting> settings)
        {
            if (settings == null || settings.Count == 0)
            {
                Debug.Log("SettingsCheckForUnapplied: Settings is null!");
                return;
            }
            
            Debug.Log("SettingsCheckForUnapplied: Unapplied settings found:");
            foreach (var setting in settings)
            {
                Debug.Log( " * " + setting.GetID());
            }
        }

        #region Editor Stuff
#if UNITY_EDITOR
        public void Reset()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            editorAutoSelectProvider();
        }

        bool _editorOnValidated;

        public void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(this.gameObject);
            if (prefabStatus != PrefabInstanceStatus.Connected)
                return;

            if (!_editorOnValidated)
            {
                _editorOnValidated = true;
                editorAutoSelectProvider();
            }
        }

        protected void editorAutoSelectProvider()
        {
            if (Provider == null)
            {
                var provider = SettingsProvider.LastUsedSettingsProvider;

                if (provider == null)
                {
                    var providerGuids = AssetDatabase.FindAssets("t:SettingsProvider");
                    if (providerGuids.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(providerGuids[0]);
                        if (path != null)
                        {
                            provider = AssetDatabase.LoadAssetAtPath<SettingsProvider>(path);
                        }
                    }
                }

                if (provider != null)
                {
                    Provider = provider;
                    EditorUtility.DisplayDialog($"Using provider \"{provider.name}\"",
                        $"SettingsCheckOnDisable has automatically chosen the provider: \"{provider.name}\".\n\n" +
                        $"If that is not the provider you want then please assign it in the inspector.",
                        "Ok");
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(this.gameObject);
                }
            }
        }
#endif
        #endregion
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(SettingsCheckForUnapplied))]
    public class SettingsCheckForUnappliedEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Please consider using the unapplied options in the settings provider instead of this component. You can still use it for events but it is recommended that you use the settings provider in the future.", MessageType.Warning);
            
            base.OnInspectorGUI();
        }
    }
#endif
}
