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

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        _timer += Time.deltaTime;
        if (_timer < _movementDuration)
        {
            transform.Translate(Vector3.forward * _movementSpeed * Time.deltaTime, Space.Self);
        }
    }
}
