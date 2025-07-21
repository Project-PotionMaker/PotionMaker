using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // This base class exists to trick the UnityEditor into
    // filtering the selection options.

    // It also handles resetting objects before play mode
    // if Domain-Reload is disabled (via IResetBeforeDomainReload).

    public abstract class ConnectionSO : ScriptableObject
#if UNITY_EDITOR
        , IResetBeforeDomainReload
#endif
    {
        public abstract void DestroyConnection();

        public abstract SettingData.DataType GetDataType();

#if UNITY_EDITOR
        // Domain Reload handling
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void onResetBeforePlayMode()
        {
            DomainReloadUtils.CallOnResetOnAssets(typeof(ConnectionSO));
        }

        public void ResetBeforePlayMode()
        {
            DestroyConnection();
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ConnectionSO), editorForChildClasses: true)]
    public class ConnectionSOEditor : UnityEditor.Editor
    {
        ConnectionSO obj;

        public void OnEnable()
        {
            obj = target as ConnectionSO;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(new GUIContent("Back to Settings", "Opens the last used settings.\n\nNOTICE: The editor may guess if there are multiple settings in the project and none was ever selected.")))
            {
                if (EditorRuntimeUtils.LastOpenedSettingsAsset != null)
                {
                    UnityEditor.Selection.objects = new Object[] { EditorRuntimeUtils.LastOpenedSettingsAsset };
                }
                else
                {
                    var provider = EditorRuntimeUtils.FindPreferredSettingsProvider();
                    if (provider != null && provider.SettingsAsset != null)
                    {
                        UnityEditor.Selection.objects = new Object[] { provider.SettingsAsset };
                    }
                }
                
            }

            base.OnInspectorGUI();
        }
    }
#endif
}
