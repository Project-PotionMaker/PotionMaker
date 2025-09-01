using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    [Header("Hierarchy")]
    [SerializeField]
    private RoomInfoHandler _roomInfoHandler;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    // 새로 추가할 콜백들
    protected Callback<LobbyMatchList_t> LobbyList; // 로비 검색 결과 리스트
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated; // 로비 데이터 업데이트

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "CustomHostAddress";

    // UI 관련 변수 (인스펙터에서 할당)
    public GameObject lobbyListContent; // 로비 목록을 담을 부모 GameObject (Vertical Layout Group 등)
    public GameObject lobbyItemPrefab; // 각 로비를 표시할 UI 프리팹 (버튼, 텍스트 포함)

    private void Start()
    {
        if (!SteamManager.Initialized)
            return;

        if (Instance == null)
            Instance = this;
        else if (Instance != this) // 인스턴스가 이미 존재하면 파괴 (싱글톤 패턴)
        {
            Destroy(gameObject);
            return;
        }

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        // 새로 추가
        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdated);

        Debug.Log($"현재 게임 Product Name: {Application.productName}");
    }

    public void OnMakeRoomButtonClick()
    {
        //Debug.Log($"{_roomInfoHandler.RoomInfo.ShopInfo.ShopName}, " +
        //    $"{_roomInfoHandler.RoomInfo.ShopInfo.Day}, " +
        //    $"{_roomInfoHandler.RoomInfo.ShopInfo.Currency}, " +
        //    $"{_roomInfoHandler.RoomInfo.Visibility}");
        MirrorNetworkManager.Instance.ShopInfo = _roomInfoHandler.ShopInfoHandler.SelectedShopInfo;
        HostLobby();
    }

    public void OnEnerRoonButtonClick()
    {
        GetLobbiesList();
    }

    // 버튼 등으로 로비 호스트할 때 실행
    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MirrorNetworkManager.Instance.maxConnections);
    }

    // 로비 참가
    public void JoinLobby(CSteamID lobbyID)
    {
        Debug.Log(lobbyID.ToString());
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    // 로비가 생성되었을 때 콜백
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("로비 생성 실패: " + callback.m_eResult);
            return;
        }

        Debug.Log("로비 생성 성공. ID: " + callback.m_ulSteamIDLobby);

        MirrorNetworkManager.Instance.StartHost();

        CSteamID newLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(newLobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(newLobbyID, "name", SteamFriends.GetPersonaName() + "'s Lobby");
        SteamMatchmaking.SetLobbyData(newLobbyID, "game", Application.productName);
        // 로비 인원수 설정 (선택 사항)
        SteamMatchmaking.SetLobbyData(newLobbyID, "maxPlayers", MirrorNetworkManager.Instance.maxConnections.ToString());
    }

    // 로비 참여 시 콜백
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("로비 참여 요청: " + callback.m_steamIDLobby.m_SteamID);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        // 만약 로비 진입자가 호스트라면, 이미 서버를 시작했으므로 클라이언트 역할만 하면 됩니다.
        // 하지만 이 경우는 오류 메시지가 클라이언트에서 뜨므로 이 조건문은 통과했을 것입니다.
        if (NetworkServer.active)
            return;

        Debug.Log($"[Client] 로비 {CurrentLobbyID}에 입장했습니다. 호스트 주소 가져오는 중...");

        // 중요: 0.1초 (또는 0.2~0.5초) 지연 후 호스트 주소 가져오기 시도
        // 이 딜레이가 데이터 동기화에 충분한 시간을 줄 수 있습니다.
        Invoke(nameof(RetrieveHostAddressAndConnect), 0.5f);
    }

    private void RetrieveHostAddressAndConnect()
    {
        // OnLobbyEntered에서 저장한 CurrentLobbyID를 사용합니다.
        // CurrentLobbyID는 ulong 타입이므로, CSteamID로 명시적 변환이 필요합니다.
        CSteamID currentLobbyCSteamID = new CSteamID(CurrentLobbyID);

        string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyCSteamID, HostAddressKey);

        if (string.IsNullOrEmpty(hostAddress))
        {
            Debug.LogError($"[Client ERROR] 로비 데이터에서 호스트 주소를 찾을 수 없습니다. " +
                           $"(Key: '{HostAddressKey}', 로비ID: {CurrentLobbyID}). " +
                           $"호스트가 해당 데이터를 설정했는지, 또는 더 많은 지연이 필요한지 확인하세요.");
            // 오류가 계속되면 사용자에게 피드백을 주거나, 재시도 로직을 고려할 수 있습니다.
            return;
        }

        MirrorNetworkManager.Instance.networkAddress = hostAddress;
        MirrorNetworkManager.Instance.StartClient();
        Debug.Log($"[Client] 로비 호스트 주소: {hostAddress}. 클라이언트 연결 시작.");
    }

    // 로비 검색 버튼에 연결할 함수
    public void GetLobbiesList()
    {
        // 기존 로비 목록 UI 삭제
        foreach (Transform child in lobbyListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // 100개의 로비까지 검색, 친구 로비만, 거리가 가까운 순
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(100);
        SteamMatchmaking.AddRequestLobbyListStringFilter("game", "potionmaker", ELobbyComparison.k_ELobbyComparisonEqual); // 게임 이름으로 필터링
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterClose);
        SteamMatchmaking.RequestLobbyList();
        Debug.Log("로비 목록을 요청합니다...");
    }

    // 로비 검색 결과 콜백
    private void OnGetLobbyList(LobbyMatchList_t callback)
    {
        Debug.Log($"로비 검색 결과: {callback.m_nLobbiesMatching}개 로비 발견.");

        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "name");
            string hostSteamID = SteamMatchmaking.GetLobbyData(lobbyID, HostAddressKey);
            string maxPlayers = SteamMatchmaking.GetLobbyData(lobbyID, "maxPlayers");

            // 로비 이름이 없으면 호스트 이름으로 대체
            if (string.IsNullOrEmpty(lobbyName))
            {
                lobbyName = "정보없음";
            }

            // UI에 로비 항목 생성
            GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbyListContent.transform);
            // lobbyItem에 포함된 UI 요소 (Text, Button 등)를 찾아서 정보 설정
            lobbyItem.GetComponentInChildren<TextMeshProUGUI>().text = $"{lobbyName} ({SteamMatchmaking.GetNumLobbyMembers(lobbyID)}/{maxPlayers})";

            // 버튼에 로비 참여 함수 연결
            Button joinButton = lobbyItem.GetComponentInChildren<Button>();
            if (joinButton != null)
            {
                // 클로저 문제를 피하기 위해 임시 변수 사용
                CSteamID currentLobbyID = lobbyID;
                joinButton.onClick.AddListener(() => JoinLobby(currentLobbyID));
            }
        }
    }

    // 로비 데이터 업데이트 콜백 (선택 사항, 로비 정보 변경 시 사용)
    private void OnLobbyDataUpdated(LobbyDataUpdate_t callback)
    {
        // 특정 로비의 데이터가 변경되었을 때 UI를 업데이트하는 등의 로직
        // 예: 로비 인원수 변경
        // if (callback.m_ulSteamIDLobby == CurrentLobbyID) { ... }
    }

    // 로비 나가기 함수 (선택 사항)
    public void LeaveLobby()
    {
        if (CurrentLobbyID != 0)
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
            CurrentLobbyID = 0; // 로비 ID 초기화
            Debug.Log("로비를 떠났습니다.");
            // NetworkManager에서 클라이언트/호스트 중지
            if (NetworkClient.isConnected)
            {
                MirrorNetworkManager.Instance.StopClient();
            }
            if (NetworkServer.active)
            {
                MirrorNetworkManager.Instance.StopHost();
            }
        }
    }
}