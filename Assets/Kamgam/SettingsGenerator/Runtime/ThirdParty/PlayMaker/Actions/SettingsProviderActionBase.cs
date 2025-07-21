#if PLAYMAKER
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// This class is the base for all actions that need to access a SettingsProvider.
    /// </summary>
#if UNITY_EDITOR
    [HelpUrl(Installer.ManualUrl)]
#endif
    public abstract class SettingsProviderActionBase : FsmStateAction
    {
        [ActionSection("Provider")]

        [ObjectType(typeof(SettingsProvider))]
        public FsmObject SettingsProvider;

        public override void OnEnter()
        {
            base.OnEnter();
            
            // If provider is null then abort.
            if (SettingsProvider == null)
            {
                Finish();
                return;
            }

            OnEnterWithProvider(SettingsProvider.Value as SettingsProvider);

            if(!Finished)
                Finish();
        }

        public abstract void OnEnterWithProvider(SettingsProvider provider);

    }
}
#endif
