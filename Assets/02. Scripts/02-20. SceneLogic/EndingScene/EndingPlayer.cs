using System;
using System.Collections;
using UnityEngine;

public class EndingPlayer : MonoBehaviour
{
    [SerializeField]
    private float _movementDuration;

    [SerializeField]
    private float _movementSpeed;

    private float _timer;

    private Animator _playerAnimator;

    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerAnimator.SetBool(nameof(EPlayerAnimationParameter.IsMove), true);
    }
    private void Start()
    {
        StartCoroutine(Coroutine_Move());
    }

    private IEnumerator Coroutine_Move()
    {
        float timer = 0f;
        while (timer < _movementDuration)
        {
            transform.Translate(Vector3.forward * _movementSpeed * Time.deltaTime, Space.Self);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void PlayAnimationWhenStopWalking()
    {
        _playerAnimator.SetBool(nameof(EPlayerAnimationParameter.IsMove), false);
        _playerAnimator.SetBool(nameof(EPlayerAnimationParameter.HasHeldItem), true);
    }

    public void PlayAnimationOnOpenDoor()
    {
        _playerAnimator.SetBool(nameof(EPlayerAnimationParameter.HasHeldItem), false);
        _playerAnimator.SetTrigger(nameof(EPlayerAnimationParameter.Ping));
    }
}
