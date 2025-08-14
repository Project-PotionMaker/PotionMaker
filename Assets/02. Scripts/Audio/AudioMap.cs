using System;
using UnityEngine;

public abstract class AudioMapBase { }

[Serializable]
public class AudioMap<T> : AudioMapBase where T : Enum
{
    [SerializeField]
    [HideInInspector]
    private AudioClip[] _clips;

    public const string ClipsFieldName = nameof(_clips);

    public AudioClip this[T type]
    {
        get => _clips[Convert.ToInt32(type)];
    } 

    public void EnsureSize()
    {
        int len = Enum.GetValues(typeof(T)).Length;
        if(_clips == null || _clips.Length != len)
        {
            Array.Resize(ref _clips, len);
        }
    }
}
