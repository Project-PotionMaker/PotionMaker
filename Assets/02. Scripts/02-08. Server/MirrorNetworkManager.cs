using Mirror;
using Mirror.Examples.MultipleMatch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorNetworkManager : NetworkRoomManager
{
    public static MirrorNetworkManager Instance => (MirrorNetworkManager)singleton;

    // netId -> UI 슬롯 번호 매핑 (0~3번만 사용)
    private Dictionary<uint, int> netIdToSlotMapping = new Dictionary<uint, int>();
    public Dictionary<uint, int> NetIdToSlotMapping => netIdToSlotMapping;
    private bool[] slotUsed = new bool[4]; // UI 슬롯 사용 여부 (0~3번)

    private ShopInfo _shopInfo;
    public ShopInfo ShopInfo { get => _shopInfo; set => _shopInfo = value; }

    [Scene]
    public string LoadingScene;

    //[Tooltip("서버 시작 시 LoadingScene에서 생성할 매니저 프리팹들")]
    //public List<GameObject> ManagerPrefabList = new List<GameObject>();

    //[Tooltip("서버 시작 시 GamePlayScene에서 생성할 팩토리 프리팹들")]
    //public List<GameObject> FactoryPrefabList = new List<GameObject>();

    public int GetSlotForNetId(uint netId)
    {
        if (netIdToSlotMapping.ContainsKey(netId))
        {
            Debug.Log($"MirrorNetworkManager: NetId {netId} already mapped to UI slot {netIdToSlotMapping[netId]}");
            return netIdToSlotMapping[netId];
        }

        // 새로운 매핑 생성 (순차적으로 할당)
        int availableSlot = GetNextAvailableSlot();
        if (availableSlot >= 0)
        {
            netIdToSlotMapping[netId] = availableSlot;
            slotUsed[availableSlot] = true;
            Debug.Log($"MirrorNetworkManager: Mapped NetId {netId} to UI slot {availableSlot}");

            // 현재 슬롯 상태 출력
            PrintSlotStatus();
        }
        else
        {
            Debug.LogWarning($"MirrorNetworkManager: No available UI slot for NetId {netId}");
        }
        return availableSlot;
    }

    private int GetNextAvailableSlot()
    {
        // 순차적으로 할당 (0, 1, 2, 3 순서)
        for (int i = 0; i < slotUsed.Length; i++)
        {
            if (!slotUsed[i])
            {
                Debug.Log($"MirrorNetworkManager: Found available slot {i}");
                return i;
            }
        }
        Debug.LogWarning("MirrorNetworkManager: No available slots found");
        return -1; // 꽉 찼을 때
    }

    public void ReleaseSlotForNetId(uint netId)
    {
        if (netIdToSlotMapping.ContainsKey(netId))
        {
            int slot = netIdToSlotMapping[netId];
            slotUsed[slot] = false;
            netIdToSlotMapping.Remove(netId);
            Debug.Log($"MirrorNetworkManager: Released UI slot {slot} for NetId {netId}");

            // 현재 슬롯 상태 출력
            PrintSlotStatus();
        }
        else
        {
            Debug.LogWarning($"MirrorNetworkManager: NetId {netId} not found in mapping");
        }
    }

    private void PrintSlotStatus()
    {
        string status = "Slot Status: ";
        for (int i = 0; i < slotUsed.Length; i++)
        {
            status += $"Slot {i}: {(slotUsed[i] ? "Used" : "Free")}, ";
        }
        Debug.Log(status);
    }

    // 모든 플레이어가 준비되면 LoadingScene으로 전환
    public override void OnRoomServerPlayersReady()
    {
        Debug.Log("서버: 모든 플레이어가 준비되었습니다. LoadingScene으로 전환합니다.");


        StartCoroutine(LoadLoadingSceneWithDelay());
    }

    // 서버에서 씬이 변경될 때 호출
    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);

        if (newSceneName == LoadingScene)
        {
            Debug.Log("서버: LoadingScene으로 전환되었습니다. 매니저 프리팹들을 스폰합니다.");
            StartCoroutine(LoadGameplaySceneWithDelay());
        }
    }

    private IEnumerator LoadLoadingSceneWithDelay()
    {
        yield return new WaitForSeconds(1.0f);

        ServerChangeScene(LoadingScene);
    }

    // 매니저 스폰이 완료될 시간을 기다린 후, GameplayScene으로 전환 명령을 보냄
    private IEnumerator LoadGameplaySceneWithDelay()
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
            if(gamePlayer.TryGetComponent<Player>(out Player playerScript))
            {
                playerScript.playerName = roomPlayerScript.PlayerName;
                playerScript.PlayerOrderIndex = roomPlayerScript.index;

                gamePlayer.transform.position = PotionHouse.Instance.Layout.PlayerSpawnPositions[playerScript.PlayerOrderIndex];
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

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);
        Debug.Log($"서버: 플레이어 {conn.connectionId}가 연결되었습니다.");
    }

    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"서버: 플레이어 {conn.connectionId}가 연결 해제되었습니다.");
        
        // 연결 해제된 플레이어의 RoomPlayer를 찾아서 슬롯 반환 및 NetworkMessenger에서 제거
        if (NetworkMessenger.Instance != null)
        {
            // 해당 연결의 RoomPlayer 찾기
            foreach (RoomPlayer roomPlayer in roomSlots)
            {
                if (roomPlayer != null && roomPlayer.connectionToClient == conn)
                {
                    // 슬롯 반환 (netId 사용)
                    ReleaseSlotForNetId(roomPlayer.netId);
                    
                    NetworkMessenger.Instance.RemovePlayerFromList(roomPlayer.netId);
                    Debug.Log($"서버: RoomPlayer {roomPlayer.netId}를 NetworkMessenger에서 제거했습니다.");
                    break;
                }
            }
        }
        
        base.OnRoomServerDisconnect(conn);
    }
}
