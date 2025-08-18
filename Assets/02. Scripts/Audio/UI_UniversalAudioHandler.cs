using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UniversalAudioHandler : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private void Awake()
    {
        if (TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener(PlayClickAudio);
        }

        if (TryGetComponent<Toggle>(out Toggle toggle))
        {
            // 값이 바뀔 때 (클릭할 때) 사운드 재생
            toggle.onValueChanged.AddListener((isOn) => PlayClickAudio());
        }

        if (TryGetComponent<InputField>(out InputField inputField))
        {
            // 입력이 끝나고 엔터를 치거나 다른 곳을 클릭했을 때 사운드 재생
            inputField.onSubmit.AddListener((text) => PlayClickAudio());
        }
    }

    private void PlaySFX(EUIAudioType audioType)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(audioType);
    }

    private void PlayClickAudio()
    {
        PlaySFX(EUIAudioType.ButtonClicked);
    }

    private void PlaySelectedAudio()
    {
        PlaySFX(EUIAudioType.ButtonSelected);
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlaySelectedAudio();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySelectedAudio();
    }
}
