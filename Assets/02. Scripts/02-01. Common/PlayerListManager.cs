using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerListManager : NetworkBehaviourSingleton<PlayerListManager>
{
    public readonly List<uint> PlayerNetIdList = new();
    public event Action OnPlayerListUpdated;

    public void AddList(uint netId)
    {
        PlayerNetIdList.Add(netId);
        OnPlayerListUpdated?.Invoke();
    }
    public void RemoveList(uint netId)
    {
        PlayerNetIdList.Remove(netId);
        OnPlayerListUpdated?.Invoke();
    }

}
