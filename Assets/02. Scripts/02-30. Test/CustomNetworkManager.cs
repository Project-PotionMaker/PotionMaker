using Mirror;
using Mirror.Examples.Common.Controllers.Player;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField]
    private PlayerObjectController GamePlayerPrefab;

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    //서버에 사람 추가될 때
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
        GamePlayerInstance.ConnectionID = conn.connectionId;
        GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
        GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

        NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
    }

    //클라이언트가 연결 끊겼을 때
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

    //서버가 연결 끊겼을 때
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }
}