using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMessenger : NetworkBehaviourSingleton<NetworkMessenger>
{
    public Action OnPlayerListUpdated;

    private PlayersInfo _playersInfo;

    public readonly SyncList<uint> RoomPlayerIdList = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("NetworkMessenger: OnStartServer");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("NetworkMessenger: OnStartClient");
        
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

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    private void OnPlayerListChanged(SyncList<uint>.Operation op, int itemIndex, uint oldItem, uint newItem)
    {
        Debug.Log($"NetworkMessenger: Player list changed - Operation: {op}, Index: {itemIndex}");
        
        // 무한 루프 방지: RpcNotifyPlayerListChanged 호출하지 않음
        // SyncList의 콜백은 이미 모든 클라이언트에서 자동으로 호출됨
        OnPlayerListUpdated?.Invoke();
    }

    [Server]
    public void AddPlayerToList(uint netId)
    {
        if (!RoomPlayerIdList.Contains(netId))
        {
            RoomPlayerIdList.Add(netId);
            Debug.Log($"NetworkMessenger: Added player {netId} to list. Total: {RoomPlayerIdList.Count}");
        }
    }

    [Server]
    public void RemovePlayerFromList(uint netId)
    {
        if (RoomPlayerIdList.Contains(netId))
        {
            RoomPlayerIdList.Remove(netId);
            Debug.Log($"NetworkMessenger: Removed player {netId} from list. Total: {RoomPlayerIdList.Count}");
        }
    }
}
