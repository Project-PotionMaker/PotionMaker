using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    public void PlayMovingSFX()
    {
        AudioManager.Instance.PlaySFX(EPlayerAudioType.Move);
    }
}
