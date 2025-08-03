using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MirrorNetworkManager : NetworkRoomManager
{
    public static MirrorNetworkManager Instance => (MirrorNetworkManager)singleton;

    [Scene]
    public string LoadingScene;

    private bool isGameplaySceneLoaded = false;

    public override void OnRoomServerPlayersReady()
    {
        base.ServerChangeScene(LoadingScene);
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        if (newSceneName == LoadingScene)
        {
            StartCoroutine(LoadGameplaySceneAsync());
        }
    }

    IEnumerator LoadGameplaySceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameplayScene, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        isGameplaySceneLoaded = true;

        Scene gameplayScene = SceneManager.GetSceneByPath(GameplayScene);
        if (gameplayScene.isLoaded)
        {
            SceneManager.SetActiveScene(gameplayScene);
        }

        yield return new WaitUntil(() => allPlayersReady);

        SpawnPlayersOnGameplayScene();

        SceneManager.UnloadSceneAsync(LoadingScene);
    }

    private void SpawnPlayersOnGameplayScene()
    {
        List<NetworkRoomPlayer> roomPlayers = new List<NetworkRoomPlayer>(roomSlots);

        foreach (var roomPlayer in roomPlayers)
        {
            if (roomPlayer != null)
            {
                Transform startPos = GetStartPosition();
                GameObject gamePlayer = startPos != null
                    ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                    : Instantiate(playerPrefab);

                NetworkServer.ReplacePlayerForConnection(roomPlayer.connectionToClient, gamePlayer, ReplacePlayerOptions.Destroy);
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        if (SceneManager.sceneCount > 1 && SceneManager.GetSceneByPath(GameplayScene).isLoaded)
        {
            Scene gameplayScene = SceneManager.GetSceneByPath(GameplayScene);
            if (gameplayScene.isLoaded)
            {
                SceneManager.SetActiveScene(gameplayScene);
            }

            SceneManager.UnloadSceneAsync(LoadingScene);
        }
    }
}