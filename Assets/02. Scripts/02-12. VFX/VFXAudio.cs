using UnityEngine;

public class VFXAudio : MonoBehaviour
{
    [SerializeField]
    private EEffectAudioType audioType;
    private void OnEnable()
    {
        AudioManager.Instance.PlaySFX(audioType);
    }
}
