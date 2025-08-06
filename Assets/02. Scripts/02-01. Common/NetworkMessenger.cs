using Mirror;
using System;
using UnityEngine;

public class NetworkMessenger : NetworkBehaviourSingleton<NetworkMessenger>
{
    public Action OnPlayerListUpdated;

    private PlayersInfo _playersInfo;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isClient)
        {
            _playersInfo = FindAnyObjectByType<PlayersInfo>();
            if(_playersInfo != null)
            {
                OnPlayerListUpdated += _playersInfo.Refresh;
            }
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isClient)
        {
            if(_playersInfo != null)
            {
                OnPlayerListUpdated -= _playersInfo.Refresh;
            }
        }
    }

    [ClientRpc]
    public void RpcNotifyPlayerListChanged()
    {
        OnPlayerListUpdated?.Invoke();
    }
}
