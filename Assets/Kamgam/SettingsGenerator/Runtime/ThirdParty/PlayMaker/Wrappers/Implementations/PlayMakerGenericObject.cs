#if PLAYMAKER
using System;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A wrapper for a generic object<br />
    /// <br />
    /// Since Playmaker variables can not store arbitrary types we have to wrap the content
    /// in a UnityEngine.Object, see: https://forum.unity.com/threads/playmaker-visual-scripting-for-unity.72349/page-70#post-9271821
    /// </summary>
    public class PlayMakerGenericObject : ScriptableObject, IEquatable<PlayMakerGenericObject>
    {
        public object Data;

        public static PlayMakerGenericObject CreateInstance(object data)
        {
            var obj = ScriptableObject.CreateInstance<PlayMakerGenericObject>();
            obj.Data = data;
            return obj;
        }

        public override bool Equals(object obj) => Equals(obj as PlayMakerGenericObject);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Data.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(PlayMakerGenericObject other)
        {
            return Data.Equals(other.Data);
        }
    }
}
#endif
