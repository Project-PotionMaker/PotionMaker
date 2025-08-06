using Mirror;
using System;
using System.Collections;
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
        Debug.Log($"OnStartClient {gameObject}");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
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
        int CurrentMaxPlayer = MirrorNetworkManager.Instance.roomSlots.Count;
        int readyPlayerNumber = 0;
        foreach(var roomPlayer in MirrorNetworkManager.Instance.roomSlots)
        {
            readyPlayerNumber += roomPlayer.readyToBegin ? 1 : 0;
        }

        if(CurrentMaxPlayer - 1 == readyPlayerNumber)
        {
            return true;
        }
        return false;
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
