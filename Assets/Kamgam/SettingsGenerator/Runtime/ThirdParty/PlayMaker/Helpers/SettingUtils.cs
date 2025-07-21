#if PLAYMAKER
namespace Kamgam.SettingsGenerator
{
    public enum SettingType
    {
        Bool = 0,
        Color = 1,
        ColorOption = 2,
        Float = 3,
        Int = 4,
        KeyCombination = 5,
        Option = 6,
        String = 7,
        Unknown = 1000,
    }

    public static class SettingUtils
    {
        public static System.Type GetSettingType(SettingType type)
        {
            switch (type)
            {
                case SettingType.Bool:
                    return typeof(SettingBool);

                case SettingType.Color:
                    return typeof(SettingColor);

                case SettingType.ColorOption:
                    return typeof(SettingColorOption);

                case SettingType.Float:
                    return typeof(SettingFloat);

                case SettingType.Int:
                    return typeof(SettingInt);

                case SettingType.KeyCombination:
                    return typeof(SettingKeyCombination);

                case SettingType.Option:
                    return typeof(SettingOption);

                case SettingType.String:
                    return typeof(SettingString);

                default:
                    return null;
            }
       }

        public static SettingType GetSettingType(this ISetting setting)
        {
            if (setting is SettingBool)
                return SettingType.Bool;

            if (setting is SettingColor)
                return SettingType.Color;

            if (setting is SettingColorOption)
                return SettingType.ColorOption;

            if (setting is SettingFloat)
                return SettingType.Float;

            if (setting is SettingInt)
                return SettingType.Int;

            if (setting is SettingKeyCombination)
                return SettingType.KeyCombination;

            if (setting is SettingOption)
                return SettingType.Option;

            if (setting is SettingString)
                return SettingType.String;

            return SettingType.Unknown;
        }
        
        public static SettingType GetSettingType(PlayMakerSettingObject obj)
        {
            return GetSettingType(obj.Setting);
        }
        
        public static SettingType GetSettingType(PlayMakerSettingsObject obj, string id)
        {
            var settings = obj.Settings;
            if (settings == null)
                return SettingType.Unknown;
            
            var setting = settings.GetSetting(id);
            if (setting == null)
                return SettingType.Unknown;
                            
            return GetSettingType(setting);
        }
    }
}
#endif