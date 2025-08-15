using UnityEngine;

public class VFXAudio : MonoBehaviour
{
    [SerializeField]
    private EVFXAudioType audioType;
    private void OnEnable()
    {
        AudioManager.Instance.PlaySFX(audioType);
    }
}
