using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAbility : PlayerAbility
{
    private Animator _animator;

    private Dictionary<EPlayerAnimationParameter, int> _parameterHashes = new Dictionary<EPlayerAnimationParameter, int>();

    protected override void Awake()
    {
        base.Awake();

        _animator = _owner.GetComponent<Animator>();

        if (_animator == null)
        {
            return;
        }

        Init();
    }

    private void Init()
    {
        foreach (EPlayerAnimationParameter animationParameter in Enum.GetValues(typeof(EPlayerAnimationParameter)))
        {
            _parameterHashes[animationParameter] = Animator.StringToHash(animationParameter.ToString());
        }
    }

    public void SetBool(EPlayerAnimationParameter animationParameter, bool isActive)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool(_parameterHashes[animationParameter], isActive);
    }

    public void SetTrigger(EPlayerAnimationParameter animationParameter)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetTrigger(_parameterHashes[animationParameter]);
    }
}
