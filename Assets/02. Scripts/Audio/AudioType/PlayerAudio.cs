using System;
using UnityEngine;

[Serializable]
public class PlayerAudio
{
    [SerializeField]
    private EPlayerAudioType _audioType;
    [SerializeField]
    private AudioClip _audioClip;
}
