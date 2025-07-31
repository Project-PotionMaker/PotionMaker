using Mirror;
using System;
using UnityEngine;

public class MirrorNetworkManager : NetworkRoomManager
{
    public static MirrorNetworkManager Instance => (MirrorNetworkManager)NetworkManager.singleton;

    //#region Unity Callbacks

    //public override void OnValidate()
    //{
    //    base.OnValidate();
    //}

    ///// <summary>
    ///// Runs on both Server and Client
    ///// Networking is NOT initialized when this fires
    ///// </summary>
    //public override void Start()
    //{
    //    base.Start();
    //}

    ///// <summary>
    ///// Runs on both Server and Client
    ///// </summary>
    //public override void LateUpdate()
    //{
    //    base.LateUpdate();
    //}

    ///// <summary>
    ///// Runs on both Server and Client
    ///// </summary>
    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //}

    //#endregion

    //#region Start & Stop

    ///// <summary>
    ///// Set the frame rate for a headless server.
    ///// <para>Override if you wish to disable the behavior or set your own tick rate.</para>
    ///// </summary>
    //public override void ConfigureHeadlessFrameRate()
    //{
    //    base.ConfigureHeadlessFrameRate();
    //}

    ///// <summary>
    ///// called when quitting the application by closing the window / pressing stop in the editor
    ///// </summary>
    //public override void OnApplicationQuit()
    //{
    //    base.OnApplicationQuit();
    //}

    //#endregion

    //#region Scene Management

    ///// <summary>
    ///// This causes the server to switch scenes and sets the networkSceneName.
    ///// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
    ///// </summary>
    ///// <param name="newSceneName"></param>
    //public override void ServerChangeScene(string newSceneName)
    //{
    //    base.ServerChangeScene(newSceneName);
    //}

    ///// <summary>
    ///// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
    ///// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
    ///// </summary>
    ///// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    //public override void OnServerChangeScene(string newSceneName) { }

    ///// <summary>
    ///// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    ///// </summary>
    ///// <param name="sceneName">The name of the new scene.</param>
    //public override void OnServerSceneChanged(string sceneName) { }

    ///// <summary>
    ///// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    ///// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    ///// </summary>
    ///// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    ///// <param name="sceneOperation">Scene operation that's about to happen</param>
    ///// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    //public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    ///// <summary>
    ///// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    ///// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    ///// </summary>
    //public override void OnClientSceneChanged()
    //{
    //    base.OnClientSceneChanged();
    //}

    //#endregion

    //#region Server System Callbacks

    ///// <summary>
    ///// Called on the server when a new client connects.
    ///// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    ///// </summary>
    ///// <param name="conn">Connection from client.</param>
    //public override void OnServerConnect(NetworkConnectionToClient conn) { }

    ///// <summary>
    ///// Called on the server when a client is ready.
    ///// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
    ///// </summary>
    ///// <param name="conn">Connection from client.</param>
    //public override void OnServerReady(NetworkConnectionToClient conn)
    //{
    //    base.OnServerReady(conn);
    //}

    ///// <summary>
    ///// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    ///// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    ///// </summary>
    ///// <param name="conn">Connection from client.</param>
    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    base.OnServerAddPlayer(conn);
    //}

    ///// <summary>
    ///// Called on the server when a client disconnects.
    ///// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
    ///// </summary>
    ///// <param name="conn">Connection from client.</param>
    //public override void OnServerDisconnect(NetworkConnectionToClient conn)
    //{
    //    base.OnServerDisconnect(conn);
    //}

    ///// <summary>
    ///// Called on server when transport raises an error.
    ///// <para>NetworkConnection may be null.</para>
    ///// </summary>
    ///// <param name="conn">Connection of the client...may be null</param>
    ///// <param name="transportError">TransportError enum</param>
    ///// <param name="message">String message of the error.</param>
    //public override void OnServerError(NetworkConnectionToClient conn, TransportError transportError, string message) { }

    ///// <summary>
    ///// Called on server when transport raises an exception.
    ///// <para>NetworkConnection may be null.</para>
    ///// </summary>
    ///// <param name="conn">Connection of the client...may be null</param>
    ///// <param name="exception">Exception thrown from the Transport.</param>
    //public override void OnServerTransportException(NetworkConnectionToClient conn, Exception exception) { }

    //#endregion

    //#region Client System Callbacks

    ///// <summary>
    ///// Called on the client when connected to a server.
    ///// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    ///// </summary>
    //public override void OnClientConnect()
    //{
    //    base.OnClientConnect();
    //}

    ///// <summary>
    ///// Called on clients when disconnected from a server.
    ///// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    ///// </summary>
    //public override void OnClientDisconnect() { }

    ///// <summary>
    ///// Called on clients when a servers tells the client it is no longer ready.
    ///// <para>This is commonly used when switching scenes.</para>
    ///// </summary>
    //public override void OnClientNotReady() { }

    ///// <summary>
    ///// Called on client when transport raises an error.</summary>
    ///// </summary>
    ///// <param name="transportError">TransportError enum.</param>
    ///// <param name="message">String message of the error.</param>
    //public override void OnClientError(TransportError transportError, string message) { }

    ///// <summary>
    ///// Called on client when transport raises an exception.</summary>
    ///// </summary>
    ///// <param name="exception">Exception thrown from the Transport.</param>
    //public override void OnClientTransportException(Exception exception) { }

    //#endregion

    //#region Start & Stop Callbacks

    //// Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
    //// their functionality, users would need override all the versions. Instead these callbacks are invoked
    //// from all versions, so users only need to implement this one case.

    ///// <summary>
    ///// This is invoked when a host is started.
    ///// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
    ///// </summary>
    //public override void OnStartHost() { }

    ///// <summary>
    ///// This is invoked when a server is started - including when a host is started.
    ///// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
    ///// </summary>
    //public override void OnStartServer() { }

    ///// <summary>
    ///// This is invoked when the client is started.
    ///// </summary>
    //public override void OnStartClient() { }

    ///// <summary>
    ///// This is called when a host is stopped.
    ///// </summary>
    //public override void OnStopHost() { }

    ///// <summary>
    ///// This is called when a server is stopped - including when a host is stopped.
    ///// </summary>
    //public override void OnStopServer() { }

    ///// <summary>
    ///// This is called when a client is stopped.
    ///// </summary>
    //public override void OnStopClient() { }

    //#endregion

    //#region Server Callbacks

    ///// <summary>
    ///// This is called on the server when the server is started - including when a host is started.
    ///// </summary>
    //public override void OnRoomStartServer() { }

    ///// <summary>
    ///// This is called on the server when the server is stopped - including when a host is stopped.
    ///// </summary>
    //public override void OnRoomStopServer() { }

    ///// <summary>
    ///// This is called on the host when a host is started.
    ///// </summary>
    //public override void OnRoomStartHost() { }

    ///// <summary>
    ///// This is called on the host when the host is stopped.
    ///// </summary>
    //public override void OnRoomStopHost() { }

    ///// <summary>
    ///// This is called on the server when a new client connects to the server.
    ///// </summary>
    ///// <param name="conn">The new connection.</param>
    //public override void OnRoomServerConnect(NetworkConnectionToClient conn) { }

    ///// <summary>
    ///// This is called on the server when a client disconnects.
    ///// </summary>
    ///// <param name="conn">The connection that disconnected.</param>
    //public override void OnRoomServerDisconnect(NetworkConnectionToClient conn) { }

    /// <summary>
    /// This is called on the server when a networked scene finishes loading.
    /// </summary>
    /// <param name="sceneName">Name of the new scene.</param>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if(sceneName == GameplayScene)
        {
            GameObject currencyManager = Instantiate(spawnPrefabs.Find(x => x.GetComponent<CurrencyManager>() != null));
            NetworkServer.Spawn(currencyManager);
        }
    }

    ///// <summary>
    ///// This allows customization of the creation of the room-player object on the server.
    ///// <para>By default the roomPlayerPrefab is used to create the room-player, but this function allows that behaviour to be customized.</para>
    ///// </summary>
    ///// <param name="conn">The connection the player object is for.</param>
    ///// <returns>The new room-player object.</returns>
    //public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    //{
    //    return base.OnRoomServerCreateRoomPlayer(conn);
    //}

    ///// <summary>
    ///// This allows customization of the creation of the GamePlayer object on the server.
    ///// <para>By default the gamePlayerPrefab is used to create the game-player, but this function allows that behaviour to be customized. The object returned from the function will be used to replace the room-player on the connection.</para>
    ///// </summary>
    ///// <param name="conn">The connection the player object is for.</param>
    ///// <param name="roomPlayer">The room player object for this connection.</param>
    ///// <returns>A new GamePlayer object.</returns>
    //public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    //{
    //    return base.OnRoomServerCreateGamePlayer(conn, roomPlayer);
    //}

    ///// <summary>
    ///// This allows customization of the creation of the GamePlayer object on the server.
    ///// <para>This is only called for subsequent GamePlay scenes after the first one.</para>
    ///// <para>See OnRoomServerCreateGamePlayer to customize the player object for the initial GamePlay scene.</para>
    ///// </summary>
    ///// <param name="conn">The connection the player object is for.</param>
    //public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    base.OnRoomServerAddPlayer(conn);
    //}

    ///// <summary>
    ///// This is called on the server when it is told that a client has finished switching from the room scene to a game player scene.
    ///// <para>When switching from the room, the room-player is replaced with a game-player object. This callback function gives an opportunity to apply state from the room-player to the game-player object.</para>
    ///// </summary>
    ///// <param name="conn">The connection of the player</param>
    ///// <param name="roomPlayer">The room player object.</param>
    ///// <param name="gamePlayer">The game player object.</param>
    ///// <returns>False to not allow this player to replace the room player.</returns>
    //public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    //{
    //    return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    //}

    ///// <summary>
    ///// This is called on server from NetworkRoomPlayer.CmdChangeReadyState when client indicates change in Ready status.
    ///// </summary>
    //public override void ReadyStatusChanged()
    //{
    //    base.ReadyStatusChanged();
    //}

    ///// <summary>
    ///// This is called on the server when all the players in the room are ready.
    ///// <para>The default implementation of this function uses ServerChangeScene() to switch to the game player scene. By implementing this callback you can customize what happens when all the players in the room are ready, such as adding a countdown or a confirmation for a group leader.</para>
    ///// </summary>
    //public override void OnRoomServerPlayersReady()
    //{
    //    base.OnRoomServerPlayersReady();
    //}

    ///// <summary>
    ///// This is called on the server when CheckReadyToBegin finds that players are not ready
    ///// <para>May be called multiple times while not ready players are joining</para>
    ///// </summary>
    //public override void OnRoomServerPlayersNotReady() { }

    //#endregion

    //#region Client Callbacks

    ///// <summary>
    ///// This is a hook to allow custom behaviour when the game client enters the room.
    ///// </summary>
    //public override void OnRoomClientEnter() { }

    ///// <summary>
    ///// This is a hook to allow custom behaviour when the game client exits the room.
    ///// </summary>
    //public override void OnRoomClientExit() { }

    ///// <summary>
    ///// This is called on the client when it connects to server.
    ///// </summary>
    //public override void OnRoomClientConnect() { }

    ///// <summary>
    ///// This is called on the client when disconnected from a server.
    ///// </summary>
    //public override void OnRoomClientDisconnect() { }

    ///// <summary>
    ///// This is called on the client when a client is started.
    ///// </summary>
    //public override void OnRoomStartClient() { }

    ///// <summary>
    ///// This is called on the client when the client stops.
    ///// </summary>
    //public override void OnRoomStopClient() { }

    ///// <summary>
    ///// This is called on the client when the client is finished loading a new networked scene.
    ///// </summary>
    //public override void OnRoomClientSceneChanged() { }

    //#endregion

    //#region Optional UI

    //public override void OnGUI()
    //{
    //    base.OnGUI();
    //}

    //#endregion
}
