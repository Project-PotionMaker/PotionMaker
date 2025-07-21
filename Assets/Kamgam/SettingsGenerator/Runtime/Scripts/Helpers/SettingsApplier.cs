using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Applies the settings in Star().<br />
    /// Add this to any scene that is not loaded additively.
    /// </summary>
    public class SettingsApplier : MonoBehaviour
    {
        public static List<SettingsApplier> Appliers = new List<SettingsApplier>();
        
        // Reset static variables on play mode enter to support disabling domain reload.
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlayModeEnter()
        {
            Appliers.Clear();
        }
#endif
        
        /// <summary>
        /// Don't forget to hook this up with the right provider in the inspector.
        /// </summary>
        public SettingsProvider Provider;

        [Header("Start")]
        public bool ApplyOnStart = true;

        [Tooltip("On start delay in seconds.")]
        public float ApplyOnStartDelay = 0f;

        [Header("Update")]
        [Tooltip("Only use this as a last resort if another system keeps overriding your settings.\n" +
            "You really should find out what system that is and route the settings through that instead of using this.")]
        public bool ApplyOnLateUpdate = false;

        [Header("Limit applied settings")]
        [Tooltip("Leave empty to apply all settings")]
        public List<string> SettingIds = new List<string>();

        public void OnEnable()
        {
            if (!Appliers.Contains(this))
                Appliers.Add(this);
        }
        
        // This is where the SettingsGeneratorSettings will try to find existing Appliers. It happens after OnEnable() but before Start().
        // see: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html

        /// <summary>
        /// Get the applier (with optional scene limitation for the search).
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static SettingsApplier GetApplier(Scene? scene = null)
        {
            foreach (var applier in Appliers)
            {
                if (applier != null && applier.gameObject.activeInHierarchy && applier.isActiveAndEnabled
                    && (!scene.HasValue || applier.gameObject.scene == scene.Value))
                {
                    return applier;
                }
            }

            return null;
        }
        
        public static SettingsApplier CreateApplier(SettingsProvider provider, Scene? scene = null)
        {
            var tmpScene = SceneManager.GetActiveScene();
            if (scene.HasValue)
                SceneManager.SetActiveScene(scene.Value);
            
            var go = new GameObject();
            go.name = "Kamgam.SettingsGenerator.SettingsApplier";
            go.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            
            go.SetActive(false);
            
            var newApplier = go.AddComponent<SettingsApplier>();
            newApplier.Provider = provider;
            newApplier.SettingIds = provider.ApplyOnSceneLoadIds;
            newApplier.ApplyOnStartDelay = provider.ApplyOnSceneLoadDelay;
            newApplier.ApplyOnLateUpdate = provider.ApplyOnSceneLoadInLateUpdate;
            
            go.SetActive(true);

            SceneManager.SetActiveScene(tmpScene);
            return newApplier;
        }

        public IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(ApplyOnStartDelay);

            if (Provider == null)
            {
                Debug.LogError("You have not set the Provider on you SettingsApplier. Please set a provider!", this);
                throw new System.Exception("Missing Provider on Settings Initializer.");
            }

            if (ApplyOnStart)
                Apply();
        }

        public void LateUpdate()
        {
            if (ApplyOnLateUpdate)
            {
                Apply();
            }
        }

        public void Apply()
        {
            if (SettingIds == null || SettingIds.Count == 0)
            {
                // Apply the settings to all connections.
                Provider.Settings.Apply(changedOnly: false);
            }
            else
            {
                // Apply only those in the settings ids.
                foreach (var id in SettingIds)
                {
                    var setting = Provider.Settings.GetSetting(id);
                    setting?.Apply();
                }
            }
        }

        public void OnDisable()
        {
            Appliers.Remove(this);
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
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            
            if (Provider == null)
            {
                var provider = EditorRuntimeUtils.FindPreferredSettingsProvider();
                if (provider != null)
                {
                    Provider = provider;
                    EditorUtility.DisplayDialog($"Using provider \"{provider.name}\"",
                        $"The SettingsApplier has automatically chosen the provider: \"{provider.name}\".\n\n" +
                        $"If that is not the provider you want then please assign it in the inspector.",
                        "Ok");
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(this.gameObject);
                }
            }
        }
#endif
        #endregion
        
        
#if UNITY_EDITOR
        [CustomEditor(typeof(SettingsApplier))]
        public class SettingsApplierEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("The SettingsApplier no longer needs to be added manually. Please consider removing it and use the configuration in the SettingsProvider instead." +
                                        "\n\nHowever, if a SettingsApplier is found in a scene then it will be used and the provider configuration will be ignored. This is done to remain backwards compatible with older versions but it's no longer recommended to use the applier." +
                                        "\n\nHINT: You can still search for Appliers during runtime (use SettingsApplier.GetApplier()) as it now will be added automatically as a temporary object if configured in the provider. So, yes, it is still used but you no longer need to add it manually.", MessageType.Warning);
            
                base.OnInspectorGUI();
            }
        }
#endif
    }
}
