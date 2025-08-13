using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public int slotNumber = -1; // UI 슬롯 번호 (0~3)

    [SyncVar(hook = nameof(OnPlayerNameChangedHook))]
    private string _playerName = "New Player";
    public string PlayerName => _playerName;

    public Action OnClientReadyStateChanged;
    public Action OnPlayerNameChanged;

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        // netId를 기준으로 UI 슬롯에 매핑
        if (MirrorNetworkManager.Instance != null)
        {
            int assignedSlot = MirrorNetworkManager.Instance.GetSlotForNetId(netId);
            if (assignedSlot >= 0)
            {
                slotNumber = assignedSlot;
                Debug.Log($"RoomPlayer: Server assigned NetId {netId} to UI slot {slotNumber}");
            }
            else
            {
                Debug.LogError($"RoomPlayer: Failed to assign slot for NetId {netId}");
            }
        }
        else
        {
            Debug.LogError("RoomPlayer: MirrorNetworkManager.Instance is null");
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        // 서버에서만 index 반환
        if (index >= 0)
        {
            MirrorNetworkManager.Instance.ReleaseSlotForNetId(netId);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"RoomPlayer: OnStartClient - Mirror Index: {index}, UI Slot: {slotNumber}, NetId: {netId}");
        
        // 서버에 플레이어 추가 요청
        if (isServer)
        {
            if (NetworkMessenger.Instance != null)
            {
                NetworkMessenger.Instance.AddPlayerToList(netId);
            }
        }
        else
        {
            CmdRequestAddToPlayerList();
            if (isOwned)
            {
                CmdGetShopInfo();
            }
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log($"RoomPlayer: OnStopClient - Mirror Index: {index}, UI Slot: {slotNumber}, NetId: {netId}");
        
        // 서버에 플레이어 제거 요청
        if (isServer)
        {
            if (NetworkMessenger.Instance != null)
            {
                NetworkMessenger.Instance.RemovePlayerFromList(netId);
            }
        }
        else if (isLocalPlayer && NetworkClient.active)
        {
            CmdRequestRemoveToPlayerList();
        }
    }

    [Command]
    private void CmdGetShopInfo()
    {
        if (connectionToClient != null)
        {
            RpcReceiveShopInfo(connectionToClient, MirrorNetworkManager.Instance.ShopInfo);
        }
    }

    [TargetRpc]
    private void RpcReceiveShopInfo(NetworkConnection target, ShopInfo shopInfo)
    {
        // 클라이언트 쪽 싱글톤 인스턴스에 ShopInfo 복원
        ShopInfoManager.Instance.ShopInfo = shopInfo;
    }

    [Command]
    void CmdRequestAddToPlayerList()
    {
        if (NetworkMessenger.Instance != null)
        {
            NetworkMessenger.Instance.AddPlayerToList(netId);
        }
    }

    [Command]
    void CmdRequestRemoveToPlayerList()
    {
        if (NetworkMessenger.Instance != null)
        {
            NetworkMessenger.Instance.RemovePlayerFromList(netId);
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetPlayerName($"플레이어 {slotNumber}");
        Debug.Log($"RoomPlayer: OnStartLocalPlayer - 플레이어 {slotNumber}");
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
        return MirrorNetworkManager.Instance.roomSlots.All(p => p.GetComponent<RoomPlayer>().slotNumber == 0 || p.readyToBegin);
    }

    [Command]
    public void CmdSetPlayerName(string newName)
    {
        _playerName = newName;
    }

    private void OnPlayerNameChangedHook(string oldName, string newName)
    {
        Debug.Log($"RoomPlayer: Name changed from {oldName} to {newName}");
        OnPlayerNameChanged?.Invoke();
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        base.ReadyStateChanged(oldReadyState, newReadyState);
        Debug.Log($"RoomPlayer: Ready state changed from {oldReadyState} to {newReadyState}");
        //OnClientReadyStateChanged?.Invoke();
        NetworkMessenger.Instance.OnPlayerListUpdated?.Invoke();
    }
}
