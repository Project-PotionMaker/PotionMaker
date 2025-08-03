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

    // LoadingScene이 로드될 때 서버에서 생성할 매니저 프리팹 리스트입니다.
    // 이 프리팹들은 NetworkBehaviour를 상속받아야 하며, NetworkIdentity 컴포넌트가 있어야 합니다.
    [Tooltip("서버 시작 시 LoadingScene에서 생성할 매니저 프리팹들")]
    public List<GameObject> managerPrefabList = new List<GameObject>();

    private bool isGameplaySceneLoaded = false;

    public override void OnStartServer()
    {
        foreach(GameObject prefab in managerPrefabList)
        {
            spawnPrefabs.Add(prefab);
        }
    }

    public override void OnRoomServerPlayersReady()
    {
        base.ServerChangeScene(LoadingScene);
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        if (newSceneName == LoadingScene)
        {
            // --- 추가된 부분: 로딩 씬에서 매니저 프리팹 생성 ---
            // 이 코드는 서버에서만 실행됩니다.
            foreach (GameObject managerPrefab in managerPrefabList)
            {
                // 프리팹을 인스턴스화합니다.
                GameObject managerInstance = Instantiate(managerPrefab);
                // NetworkServer.Spawn()을 호출하여 이 오브젝트를 네트워크 오브젝트로 만들고,
                // 모든 클라이언트에게 스폰하라는 명령을 보냅니다.
                // 이 오브젝트는 자동으로 DontDestroyOnLoad 상태가 되어 씬 전환 시 파괴되지 않습니다.
                NetworkServer.Spawn(managerInstance);
            }
            // ----------------------------------------------------

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

        NetworkServer.SpawnObjects();
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
