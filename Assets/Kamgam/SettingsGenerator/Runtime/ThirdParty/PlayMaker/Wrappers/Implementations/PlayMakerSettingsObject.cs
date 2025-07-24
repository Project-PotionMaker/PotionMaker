#if PLAYMAKER
using System;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A wrapper for Settings<br />
    /// <br />
    /// Since Playmaker variables can not store arbitrary types we have to wrap VisualElements
    /// in a UnityEngine.Object, see: https://forum.unity.com/threads/playmaker-visual-scripting-for-unity.72349/page-70#post-9271821
    /// </summary>
    public class PlayMakerSettingsObject : ScriptableObject, IEquatable<PlayMakerSettingsObject>
    {
        protected Settings _settings;
        public Settings Settings
        {
            get => _settings;

            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    refreshName();
                }
            }
        }

        public static PlayMakerSettingsObject CreateInstance(Settings settings)
        {
            var obj = ScriptableObject.CreateInstance<PlayMakerSettingsObject>();
            obj.Settings = settings;
            return obj;
        }

        protected void refreshName()
        {
            if (!string.IsNullOrEmpty(Settings.name))
            {
                name = Settings.name;
            }
            else
            {
                name = Settings.GetType().Name;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlayMakerSettingsObject);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Settings.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(PlayMakerSettingsObject other)
        {
            return Settings.Equals(other.Settings);
        }
    }
}
#endif
