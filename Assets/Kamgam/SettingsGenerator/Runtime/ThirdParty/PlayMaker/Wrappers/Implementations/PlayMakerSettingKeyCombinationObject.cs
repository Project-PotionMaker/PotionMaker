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
    public class PlayMakerSettingKeyCombinationObject : ScriptableObject, IEquatable<PlayMakerSettingKeyCombinationObject>
    {
        protected KeyCombination _keyCombination;
        public KeyCombination KeyCombination
        {
            get => _keyCombination;

            set
            {
                _keyCombination = value;
                refreshName();
            }
        }

        public static PlayMakerSettingKeyCombinationObject CreateInstance(KeyCombination keyCombo)
        {
            var obj = ScriptableObject.CreateInstance<PlayMakerSettingKeyCombinationObject>();
            obj.KeyCombination = keyCombo;
            return obj;
        }

        protected void refreshName()
        {
            if (!string.IsNullOrEmpty(KeyCombination.Key.ToString()))
            {
                name = KeyCombination.Key.ToString();
            }
            else
            {
                name = KeyCombination.GetType().Name;
            }
        }

        public override bool Equals(object obj) => Equals(obj as PlayMakerSettingKeyCombinationObject);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = KeyCombination.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(PlayMakerSettingKeyCombinationObject other)
        {
            return KeyCombination.Equals(other.KeyCombination);
        }
    }
}
#endif
