using Mirror;
using System;
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
    }

    private void Update()
    {
        if(isLocalPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            CmdChangeReadyState(!readyToBegin);
        }
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
