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

    //public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    //{

    //}

    private void OnPlayerNameChangedHook(string oldName, string newName)
    {
        OnPlayerNameChanged?.Invoke();
    }




    public override void OnClientEnterRoom()
    {
        //Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
    }

    public override void OnClientExitRoom()
    {
        //Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
    }

    public override void IndexChanged(int oldIndex, int newIndex)
    {
        //Debug.Log($"IndexChanged {newIndex}");
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        //Debug.Log($"ReadyStateChanged {newReadyState}");
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }
}
