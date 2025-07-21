#if PLAYMAKER
using HutongGames.PlayMaker;

namespace Kamgam.SettingsGenerator
{
    public static class ObjExtensions
    {
        public static bool HasValue(this FsmObject obj)
        {
            return obj != null && obj.Value != null;
        }

        public static bool HasValue<T>(this FsmObject obj) where T : class
        {
            if (obj == null || obj.Value == null)
                return false;

            var typedValue = obj as T;
            if (typedValue != null)
                return true;

            return false;
        }

        public static bool HasValue(this PlayMakerSettingObject obj)
        {
            return obj != null && obj.Setting != null;
        }
        
        public static bool HasValue(this PlayMakerSettingsObject obj)
        {
            return obj != null && obj.Settings != null;
        }
        
        public static bool HasValue(this PlayMakerSettingsProviderObject obj)
        {
            return obj != null && obj.SettingsProvider != null;
        }

        public static T GetWrapper<T>(this FsmObject obj) where T : class
        {
            if (obj == null || obj.Value == null)
                return null;

            var typedValue = obj as T;
            if (typedValue != null)
                return typedValue;

            return null;
        }

        public static bool TryGetWrapper<T>(this FsmObject obj, out T value) where T : class
        {
            if (obj == null || obj.Value == null)
            {
                value = null;
                return false;
            }

            var typedValue = obj as T;
            if (typedValue != null)
            {
                value = typedValue;
                return true;
            }

            value = null;
            return false;
        }

        public static bool TryGetSetting(this FsmObject fsmObj, out ISetting setting)
        {
            if (!fsmObj.HasValue())
            {
                setting = null;
                return false;
            }

            var wrapper = fsmObj.Value as PlayMakerSettingObject;
            if (!wrapper.HasValue())
            {
                setting = null;
                return false;
            }

            setting = wrapper.Setting;
            return true;
        }
        
        public static bool TryGetSettings(this FsmObject fsmObj, out Settings settings)
        {
            if (!fsmObj.HasValue())
            {
                settings = null;
                return false;
            }

            var wrapper = fsmObj.Value as PlayMakerSettingsObject;
            if (!wrapper.HasValue())
            {
                settings = null;
                return false;
            }

            settings = wrapper.Settings;
            return true;
        }
        
        public static bool TryGetSettingsProvider(this FsmObject fsmObj, out SettingsProvider settings)
        {
            if (!fsmObj.HasValue())
            {
                settings = null;
                return false;
            }

            var wrapper = fsmObj.Value as PlayMakerSettingsProviderObject;
            if (!wrapper.HasValue())
            {
                settings = null;
                return false;
            }

            settings = wrapper.SettingsProvider;
            return true;
        }
    }
}
#endif
