#if PLAYMAKER
using HutongGames.PlayMaker;
using UnityEngine;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace Kamgam.SettingsGenerator
{
    [ActionCategory("Setting Generator")]
#if UNITY_EDITOR
    [HelpUrl(Installer.ManualUrl)]
#endif
    public class SettingsDebugLogSetting : FsmStateAction
    {
        [ActionSection("Setting Source")]

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Source of the Setting.")]
        public FsmObject Setting;

        public override void OnEnter()
        {
            if (Setting.TryGetSetting(out var setting))
            {
                Debug.Log(setting.GetID() + ": " + setting.GetValueAsObject());
            }

            Finish();
        }
    }
}
#endif
