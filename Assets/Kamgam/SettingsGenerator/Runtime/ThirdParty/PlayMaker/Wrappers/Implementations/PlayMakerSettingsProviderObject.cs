#if PLAYMAKER
using System;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A wrapper for a SettingsProvider<br />
    /// <br />
    /// Since Playmaker variables can not store arbitrary types we have to wrap VisualElements
    /// in a UnityEngine.Object, see: https://forum.unity.com/threads/playmaker-visual-scripting-for-unity.72349/page-70#post-9271821
    /// </summary>
    public class PlayMakerSettingsProviderObject : ScriptableObject, IEquatable<PlayMakerSettingsProviderObject>
    {
        protected SettingsProvider _settingsProvider;
        public SettingsProvider SettingsProvider
        {
            get => _settingsProvider;

            set
            {
                if (_settingsProvider != value)
                {
                    _settingsProvider = value;
                    refreshName();
                }
            }
        }

        public static PlayMakerSettingsProviderObject CreateInstance(SettingsProvider provider)
        {
            var obj = ScriptableObject.CreateInstance<PlayMakerSettingsProviderObject>();
            obj.SettingsProvider = provider;
            return obj;
        }

        protected void refreshName()
        {
            if (!string.IsNullOrEmpty(SettingsProvider.name))
            {
                name = SettingsProvider.name;
            }
            else
            {
                name = SettingsProvider.GetType().Name;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlayMakerSettingsProviderObject);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = SettingsProvider.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(PlayMakerSettingsProviderObject other)
        {
            return SettingsProvider.Equals(other.SettingsProvider);
        }
    }
}
#endif
