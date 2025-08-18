using Mirror;
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

        CmdRequestPing();
    }

    [Command]
    private void CmdRequestPing()
    {
        Vector3 position = _owner.GetFrontPosition();

        GameObject pingObject = VFXFactory.Instance.CreateObject(EVFXType.Ping, position, Quaternion.identity);
        if (pingObject.TryGetComponent<VFXColorHandler>(out VFXColorHandler ping))
        {
            ping.RpcChangeVFXColor(_owner.PlayerOrderIndex);
        }
    }

    private void OnDestroy()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnPingEvent -= Ping;
        }
    }
}
