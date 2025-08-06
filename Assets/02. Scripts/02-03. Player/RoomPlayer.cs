using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnPlayerNameChangedHook))]
    private string _playerName = "New Player";
    public string PlayerName => _playerName;

    public Action OnClientReadyStateChanged;
    public Action OnPlayerNameChanged;

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdRequestAddToPlayerList();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        CmdRequestRemoveToPlayerList();
    }

    [Command]
    void CmdRequestAddToPlayerList()
    {
        NetworkMessenger.Instance.RoomPlayerIdList.Add(netId);
    }

    [Command]
    void CmdRequestRemoveToPlayerList()
    {
        NetworkMessenger.Instance.RoomPlayerIdList.Remove(netId);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetPlayerName($"플레이어 {index}");
        Debug.Log($"플레이어 {index}");
    }

    private void Update()
    {
        if(isLocalPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            if (NetworkServer.active)
            {
                if (CheckPlayersReadyForHost())
                {
                    CmdChangeReadyState(true);
                }
            }
            else CmdChangeReadyState(!readyToBegin);
        }
    }

    public bool CheckPlayersReadyForHost()
    {
        return MirrorNetworkManager.Instance.roomSlots.All(p => p.index == 0 || p.readyToBegin);
    }

    [Command]
    public void CmdSetPlayerName(string newName)
    {
        _playerName = newName;
    }

    private void OnPlayerNameChangedHook(string oldName, string newName)
    {
        OnPlayerNameChanged?.Invoke();
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        base.ReadyStateChanged(oldReadyState, newReadyState);
        OnClientReadyStateChanged?.Invoke();
    }
}
