#if PLAYMAKER
using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A wrapper for a Setting<br />
    /// <br />
    /// Since Playmaker variables can not store arbitrary types we have to wrap VisualElements
    /// in a UnityEngine.Object, see: https://forum.unity.com/threads/playmaker-visual-scripting-for-unity.72349/page-70#post-9271821
    /// </summary>
    public class PlayMakerSettingObject : ScriptableObject, IEquatable<PlayMakerSettingObject>
    {
        protected ISetting _setting;
        public ISetting Setting
        {
            get => _setting;

            set
            {
                if (_setting != value)
                {
                    _setting = value;
                    refreshName();
                }
            }
        }

        public static PlayMakerSettingObject CreateInstance(ISetting setting)
        {
            var obj = ScriptableObject.CreateInstance<PlayMakerSettingObject>();
            obj.Setting = setting;
            return obj;
        }

        protected void refreshName()
        {
            if (!string.IsNullOrEmpty(Setting.GetID()))
            {
                name = Setting.GetID();
            }
            else
            {
                name = Setting.GetType().Name;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlayMakerSettingObject);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Setting.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(PlayMakerSettingObject other)
        {
            return Setting.Equals(other.Setting);
        }
    }
}
#endif
