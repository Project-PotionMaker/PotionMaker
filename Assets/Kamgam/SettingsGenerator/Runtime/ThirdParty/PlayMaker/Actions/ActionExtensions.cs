#if PLAYMAKER
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace Kamgam.SettingsGenerator
{
    public static class ActionExtensions
    {
        /// <summary>
        /// Used to set the inner values of a custom type wrapper.
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="setting"></param>
        /// <param name="reuseResultVariable"></param>
        public static void SetResultSetting(this FsmObject Result, ISetting setting, bool reuseResultVariable)
        {
            if (reuseResultVariable && Result != null && Result.Value != null)
            {
                PlayMakerSettingObject wrapper = Result.Value as PlayMakerSettingObject;
                if (wrapper != null)
                {
                    wrapper.Setting = setting;
                    return;
                }
            }

            Result.Value = PlayMakerSettingObject.CreateInstance(setting);
        }
        
        public static void SetResultSettings(this FsmObject Result, Settings settings, bool reuseResultVariable)
        {
            if (reuseResultVariable && Result != null && Result.Value != null)
            {
                PlayMakerSettingsObject wrapper = Result.Value as PlayMakerSettingsObject;
                if (wrapper != null)
                {
                    wrapper.Settings = settings;
                    return;
                }
            }

            Result.Value = PlayMakerSettingsObject.CreateInstance(settings);
        }
        
        public static void SetResultSettingProvider(this FsmObject Result, SettingsProvider settingsProvider, bool reuseResultVariable)
        {
            if (reuseResultVariable && Result != null && Result.Value != null)
            {
                PlayMakerSettingsProviderObject wrapper = Result.Value as PlayMakerSettingsProviderObject;
                if (wrapper != null)
                {
                    wrapper.SettingsProvider = settingsProvider;
                    return;
                }
            }

            Result.Value = PlayMakerSettingsProviderObject.CreateInstance(settingsProvider);
        }

        public static void SetResultGeneric(this FsmObject Result, object data, bool reuseResultVariable)
        {
            if (reuseResultVariable && Result != null && Result.Value != null)
            {
                PlayMakerGenericObject wrapper = Result.Value as PlayMakerGenericObject;
                if (wrapper != null)
                {
                    wrapper.Data = data;
                    return;
                }
            }

            Result.Value = PlayMakerGenericObject.CreateInstance(data);
        }
    }
}
#endif
