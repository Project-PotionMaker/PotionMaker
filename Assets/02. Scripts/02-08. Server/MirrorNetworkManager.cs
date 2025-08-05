using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorNetworkManager : NetworkRoomManager
{
    public static MirrorNetworkManager Instance => (MirrorNetworkManager)singleton;

    //private List<>

    [Scene]
    public string LoadingScene;

    [Tooltip("서버 시작 시 LoadingScene에서 생성할 매니저 프리팹들")]
    public List<GameObject> ManagerPrefabList = new List<GameObject>();

    [Tooltip("서버 시작 시 GamePlayScene에서 생성할 팩토리 프리팹들")]
    public List<GameObject> FactoryPrefabList = new List<GameObject>();

    public Action OnWaitingScenePlayerAdded;

    // 모든 플레이어가 준비되면 LoadingScene으로 전환
    public override void OnRoomServerPlayersReady()
    {
        Debug.Log("서버: 모든 플레이어가 준비되었습니다. LoadingScene으로 전환합니다.");
        ServerChangeScene(LoadingScene);
    }

    // 서버에서 씬이 변경될 때 호출
    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        if (newSceneName == LoadingScene)
        {
            Debug.Log("서버: LoadingScene으로 전환되었습니다. 매니저 프리팹들을 스폰합니다.");

            // LoadingScene에서 필요한 매니저 프리팹들을 스폰
            foreach (GameObject prefab in ManagerPrefabList)
            {
                GameObject obj = Instantiate(prefab);
                NetworkServer.Spawn(obj);
            }

            // 매니저 스폰이 완료되면 GameplayScene으로 전환하라고 명령
            // 클라이언트도 이 명령을 받아 GameplayScene을 로드
            StartCoroutine(LoadGameplaySceneWithDelay());
        }

        if (newSceneName == GameplayScene)
        {
            Debug.Log("서버: GameplayScene으로 전환되었습니다. 팩토리 프리팹들을 스폰합니다.");

            // GameplayScene에서 필요한 팩토리 프리팹들을 스폰
            foreach (GameObject prefab in FactoryPrefabList)
            {
                GameObject obj = Instantiate(prefab);
                NetworkServer.Spawn(obj);
            }
        }
    }

    // 매니저 스폰이 완료될 시간을 기다린 후, GameplayScene으로 전환 명령을 보냄
    IEnumerator LoadGameplaySceneWithDelay()
    {
        // 프리팹 스폰 후 최소 1초 대기 (네트워크 동기화 시간 확보)
        yield return new WaitForSeconds(1.0f);

        Debug.Log("서버: GameplayScene으로 전환 명령을 보냅니다.");
        ServerChangeScene(GameplayScene);
    }

    // 이 함수는 서버에서 GameplayScene 로드가 완료된 후, 각 플레이어마다 호출
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        Debug.Log($"서버: 플레이어 {conn.connectionId}의 씬 로드 완료. RoomPlayer를 GamePlayer로 교체합니다.");

        if(roomPlayer.TryGetComponent<RoomPlayer>(out RoomPlayer roomPlayerScript))
        {
            if(gameObject.TryGetComponent<Player>(out Player playerScript))
            {
                playerScript.playerName = roomPlayerScript.PlayerName;
                playerScript.playerOrderIndex = roomPlayerScript.index;
            }
        }

        // NetworkServer.ReplacePlayerForConnection()은 roomPlayer를 파괴
        NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, ReplacePlayerOptions.Destroy);

        // 추가적인 안전장치: 혹시라도 남아있을 roomPlayer를 명시적으로 파괴
        if (roomPlayer != null)
        {
            NetworkServer.Destroy(roomPlayer);
            Debug.Log($"서버: RoomPlayer 객체 {roomPlayer.name} 명시적으로 파괴");
        }

        return true;
    }

    // 클라이언트에서 씬이 변경될 때 호출
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        Debug.Log("클라이언트: 씬 변경됨 - " + SceneManager.GetActiveScene().name);
    }

    public override void OnRoomClientConnect()
    {
        base.OnRoomClientConnect();

        StartCoroutine(WaitAndRefreshRoomPlayers());
    }

    private IEnumerator WaitAndRefreshRoomPlayers()
    {
        yield return new WaitForSeconds(0.1f); // 1프레임 쉬고

        OnWaitingScenePlayerAdded?.Invoke();
    }
}
