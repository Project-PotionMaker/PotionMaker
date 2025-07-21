#if PLAYMAKER
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace Kamgam.SettingsGenerator
{
    [ActionCategory("Setting Generator")]
#if UNITY_EDITOR
    [HelpUrl(Installer.ManualUrl)]
#endif
    public class SettingGetKeyCombinationValue : FsmStateAction
    {
        [ActionSection("Setting Source")]

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Source of the Setting.")]
        public FsmObject Setting;
        
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Contains the value of the setting as a PlayMakerSettingKeyCombinationObject.")]
        public FsmObject KeyCombinationValue;
        
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Contains the value of the setting key as a int (KeyCode).")]
        public FsmInt Key;
        
        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Contains the value of the setting key as a int (KeyCode).")]
        public FsmInt ModifierKey;

        public override void OnEnter()
        {
            if (Setting.TryGetSetting(out var setting))
            {
                KeyCombinationValue.Value = PlayMakerSettingKeyCombinationObject.CreateInstance(setting.GetKeyCombinationValue());
                Key.Value = (int)setting.GetKeyCombinationValue().Key;
                ModifierKey.Value = (int)setting.GetKeyCombinationValue().ModifierKey;
            }

            Finish();
        }
    }
}
#endif
