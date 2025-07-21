#if PLAYMAKER
using System;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A wrapper for a generic struct<br />
    /// <br />
    /// Since Playmaker variables can not store arbitrary types we have to wrap the content
    /// in a UnityEngie.Object, see: https://forum.unity.com/threads/playmaker-visual-scripting-for-unity.72349/page-70#post-9271821
    /// And: https://hutonggames.com/playmakerforum/index.php?topic=3996.0
    /// </summary>
    public abstract class GenericStructObject<T> : ScriptableObject, IEquatable<GenericStructObject<T>>
        where T : struct, IEquatable<T>
    {
        public T Data;

        public override bool Equals(object obj) => Equals(obj as GenericStructObject<T>);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Data.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(GenericStructObject<T> other)
        {
            return Data.Equals(other.Data);
        }
    }
}
#endif
