#if PLAYMAKER
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace Kamgam.SettingsGenerator
{
    [ActionCategory("Setting Generator")]
#if UNITY_EDITOR
    [HelpUrl(Installer.ManualUrl)]
#endif
    public class SettingGetBoolValue : FsmStateAction
    {
        [ActionSection("Setting Source")]

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Source of the Setting.")]
        public FsmObject Setting;
        
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Contains the value of the setting.")]
        public FsmBool BoolValue;

        public override void OnEnter()
        {
            if (Setting.TryGetSetting(out var setting))
            {
                BoolValue.Value = setting.GetBoolValue();
            }

            Finish();
        }
    }
}
#endif
