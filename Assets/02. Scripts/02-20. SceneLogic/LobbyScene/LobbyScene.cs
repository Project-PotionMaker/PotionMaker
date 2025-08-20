using Mirror.Discovery;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private RoomInfoHandler _roomInfoHandler;

    private void Start()
    {
        AudioManager.Instance.PlayBGM(EBGMAudioType.Lobby);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //public void OnMakeRoomButtonClick()
    //{
    //    MirrorNetworkManager.Instance.ShopInfo = _roomInfoHandler.ShopInfoHandler.SelectedShopInfo;
    //    MirrorNetworkManager.Instance.StartHost();
    //}

    //public void OnEnerRoonButtonClick()
    //{
    //    MirrorNetworkManager.Instance.StartClient();
    //}

    public void StartDiscovery()
    {
        RoomDiscovery.Instance.StartDiscovery();
    }

    public void OnCreateRoom()
    {
        MirrorNetworkManager.Instance.ShopInfo = _roomInfoHandler.ShopInfoHandler.SelectedShopInfo;
        // 1. Host 시작
        MirrorNetworkManager.Instance.StartHost();

        // 2. Discovery 광고 시작
        RoomDiscovery.Instance.AdvertiseServer();

        // 3. 코드 생성
        string roomCode = ServerCodeGenerator.ToRoomCode(RoomDiscovery.Instance.ServerId);
        MirrorNetworkManager.Instance.SetRoomCode(roomCode);
    }
}