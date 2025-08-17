using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using System;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    private const string BGM_KEY = "BGM";
    private const string SFX_KEY = "SFX";

    [SerializeField]
    private AudioMixer _mixer;
    [SerializeField]
    private AudioMixerGroup _bgmMixerGroup;
    [SerializeField]
    private AudioMixerGroup _sfxMixerGroup;

    [SerializeField]
    private AudioMap<EBGMAudioType> _BGMAudio;
    [SerializeField]
    private AudioMap<EEffectAudioType> _EffectAudio;
    [SerializeField]
    private AudioMap<EPlayerAudioType> _playerAudio;
    [SerializeField]
    private AudioMap<EMachineAudioType> _machineAudio;
    [SerializeField]
    private AudioMap<EStorageAudioType> _storageAudio;
    [SerializeField]
    private AudioMap<ECustomerAudioType> _customerAudio;
    [SerializeField]
    private AudioMap<EPhaseAudioType> _phaseAudio;
    [SerializeField]
    private AudioMap<EUIAudioType> _UIAudio;
    [SerializeField]
    private AudioMap<EPopupAudioType> _popupAudio;

    [Header("Audio Pool")]
    public int poolSize = 20;
    private List<AudioSource> audioSourceList = new List<AudioSource>();

    public GameObject AudioSourceChildObject;

    public AudioSource BGMAudioSource;
    private Coroutine fadeCoroutine;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < poolSize; i++)
        {
            var source = Instantiate(AudioSourceChildObject, transform.position, Quaternion.identity, gameObject.transform).GetComponent<AudioSource>();
            audioSourceList.Add(source);
        }
        WarmUpBGM();
        BGMAudioSource.resource = _BGMAudio[EBGMAudioType.Lobby];
        BGMAudioSource.Play();
    }

    public void WarmUpBGM()
    {
        foreach(EBGMAudioType BGMAudioType in Enum.GetValues(typeof(EBGMAudioType)))
        {
            BGMAudioSource.resource = _BGMAudio[BGMAudioType];
            BGMAudioSource.Play();
            BGMAudioSource.Stop();
        }
    }

    public void SetBGMVolume(float sliderValue)
    {
        float volume = Mathf.Log10(sliderValue <= 0.001f ? 0.001f : sliderValue) * 20;
        _mixer.SetFloat(BGM_KEY, volume);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volume = Mathf.Log10(sliderValue <= 0.001f ? 0.001f : sliderValue) * 20;
        _mixer.SetFloat(SFX_KEY, volume);
    }

    public void PlayBGM(EBGMAudioType BGMAudioType, float fadeTime = 2f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(ChangeBGMRoutine(BGMAudioType, fadeTime));
    }

    private IEnumerator ChangeBGMRoutine(EBGMAudioType BGMAudioType, float fadeTime)
    {
        float currentVolume;
        _mixer.GetFloat(BGM_KEY, out currentVolume);

        // 1. 볼륨 줄이기
        yield return StartCoroutine(SetMixerVolume(BGM_KEY, currentVolume, -80f, fadeTime / 2f));

        // 2. 클립 교체 후 재생
        BGMAudioSource.resource = _BGMAudio[BGMAudioType];
        BGMAudioSource.Play();

        // 3. 볼륨 다시 올리기
        yield return StartCoroutine(SetMixerVolume(BGM_KEY, -80f, currentVolume, fadeTime / 2f));
    }

    private IEnumerator SetMixerVolume(string exposedParam, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float value = Mathf.Lerp(from, to, elapsed / duration);
            _mixer.SetFloat(exposedParam, value);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _mixer.SetFloat(exposedParam, to);
    }

    public void PlaySFX(EEffectAudioType audioType) => PlaySFXInternal(_EffectAudio, audioType);
    public void PlaySFX(EPlayerAudioType audioType) => PlaySFXInternal(_playerAudio, audioType);
    public void PlaySFX(EMachineAudioType audioType) => PlaySFXInternal(_machineAudio, audioType);
    public void PlaySFX(EStorageAudioType audioType) => PlaySFXInternal(_storageAudio, audioType);
    public void PlaySFX(ECustomerAudioType audioType) => PlaySFXInternal(_customerAudio, audioType);
    public void PlaySFX(EPhaseAudioType audioType) => PlaySFXInternal(_phaseAudio, audioType);
    public void PlaySFX(EUIAudioType audioType) => PlaySFXInternal(_UIAudio, audioType);
    public void PlaySFX(EPopupAudioType audioType) => PlaySFXInternal(_popupAudio, audioType);

    private void PlaySFXInternal<T>(AudioMap<T> audioMap, T audioType) where T:Enum
    {
        AudioSource audioSource = GetAvailableAudioSource();
        audioSource.outputAudioMixerGroup = _sfxMixerGroup;
        audioSource.resource = audioMap[audioType];
        audioSource.Play();
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourceList)
        {
            if (!source.isPlaying) return source;
        }

        var newSource = Instantiate(AudioSourceChildObject, transform.position, Quaternion.identity, gameObject.transform).GetComponent<AudioSource>();
        audioSourceList.Add(newSource);
        return newSource;
    }
}