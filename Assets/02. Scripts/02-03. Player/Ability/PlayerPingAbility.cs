using UnityEngine;

public class PlayerPingAbility : PlayerAbility
{
    private PlayerAnimationAbility _animationAbility;

    private void Start()
    {
        if (!_owner.isLocalPlayer)
        {
            return;
        }

        InputManager.Instance.OnPingEvent += Ping;
        _animationAbility = _owner.GetAbility<PlayerAnimationAbility>();
    }

    private void Ping()
    {
        _animationAbility.SetTrigger(EPlayerAnimationParameter.Ping);
    }
}
