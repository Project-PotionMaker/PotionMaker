using System;
using UnityEngine;

[Serializable]
public class MachineAudio
{
    [SerializeField]
    private EMachineAudioType _audioType;
    [SerializeField]
    private AudioClip _audioClip;
}
