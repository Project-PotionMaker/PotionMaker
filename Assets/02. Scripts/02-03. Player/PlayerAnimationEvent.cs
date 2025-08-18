using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void PlayMovingSFX()
    {
    //    if (_animator.IsInTransition(0))
    //    {
    //        Debug.Log("트랜지션 중");
    //        return;
    //    }
        AudioManager.Instance.PlaySFX(EPlayerAudioType.Move);
    }
}
