using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMessenger : NetworkBehaviourSingleton<NetworkMessenger>
{
    public Action OnPlayerListUpdated;

    private PlayersInfo _playersInfo;

    public SyncList<uint> RoomPlayerIdList = new();

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

            RoomPlayerIdList.Callback += OnPlayerListChanged;
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

            RoomPlayerIdList.Callback -= OnPlayerListChanged;
        }
    }

    [ClientRpc]
    public void RpcNotifyPlayerListChanged()
    {
        OnPlayerListUpdated?.Invoke();
    }

    public void NotifyAddPlayer(NetworkConnectionToClient conn)
    {
        RpcNotifyPlayerListChanged();
    }

    private void OnPlayerListChanged(SyncList<uint>.Operation op, int itemIndex, uint oldItem, uint newItem)
    {
        NetworkMessenger.Instance.RpcNotifyPlayerListChanged();
    }
}
