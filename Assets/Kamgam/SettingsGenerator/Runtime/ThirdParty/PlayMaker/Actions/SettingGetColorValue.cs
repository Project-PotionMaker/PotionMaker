#if PLAYMAKER
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace Kamgam.SettingsGenerator
{
    [ActionCategory("Setting Generator")]
#if UNITY_EDITOR
    [HelpUrl(Installer.ManualUrl)]
#endif
    public class SettingGetColorValue : FsmStateAction
    {
        [ActionSection("Setting Source")]

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Source of the Setting.")]
        public FsmObject Setting;
        
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Contains the value of the setting.")]
        public FsmColor ColorValue;

        public override void OnEnter()
        {
            if (Setting.TryGetSetting(out var setting))
            {
                ColorValue.Value = setting.GetColorValue();
            }

            Finish();
        }
    }
}
#endif
