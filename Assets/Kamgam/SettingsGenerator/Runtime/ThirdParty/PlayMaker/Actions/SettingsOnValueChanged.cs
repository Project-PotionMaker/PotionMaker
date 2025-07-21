#if PLAYMAKER
using HutongGames.PlayMaker;

namespace Kamgam.SettingsGenerator
{
    [ActionCategory("Setting Generator")]
#if UNITY_EDITOR
    [HelpUrl(Installer.ManualUrl)]
#endif
    public class SettingsOnValueChanged : SettingsProviderActionBase
    {
        [ActionSection("Event")]

        [TooltipAttribute("The id of the setting.")]
        public FsmString SettingId;
        
        [TooltipAttribute("The event that should be triggered on value changed.")]
        public FsmEvent OnValueChangedEvent;

        [UIHint(UIHint.Variable)]
        [TooltipAttribute("Contains the data of the last event that was triggered.")]
        public FsmObject StoreEventData;

        protected bool _triggered = false;

        protected SettingsProvider _provider;

        public override void OnEnterWithProvider(SettingsProvider provider)
        {
            if (_triggered)
                return;

            // Trigger settings load if needed.
            var _ = provider.Settings;
            
            _provider = provider;

            _provider.Settings.OnSettingChanged += onSettingChanged;

            Finish();
        }

        protected void onSettingChanged(ISetting setting)
        {
            if (setting.GetID() != SettingId.Value)
                return;
            
            var obj = PlayMakerSettingObject.CreateInstance(setting);
            
            // Store in variable
            if (StoreEventData != null)
                StoreEventData.Value = obj;

            // Trigger event
            Fsm.Event(OnValueChangedEvent);
        }

        public override void OnExit()
        {
            _provider.Settings.OnSettingChanged -= onSettingChanged;
            base.OnExit();
        }
    }
}
#endif