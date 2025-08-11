using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private RoomInfoHandler _roomInfoHandler;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnMakeRoomButtonClick()
    {
        //Debug.Log($"{_roomInfoHandler.RoomInfo.ShopInfo.ShopName}, " +
        //    $"{_roomInfoHandler.RoomInfo.ShopInfo.Day}, " +
        //    $"{_roomInfoHandler.RoomInfo.ShopInfo.Currency}, " +
        //    $"{_roomInfoHandler.RoomInfo.Visibility}");

        MirrorNetworkManager.Instance.StartHost();
    }

    public void OnEnerRoonButtonClick()
    {
        MirrorNetworkManager.Instance.StartClient();
    }
}
