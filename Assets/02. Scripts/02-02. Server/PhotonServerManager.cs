using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// 역할 : 포톤 서버 관리자(서버 연결, 로비 입장, 방 입장, 게임 입장)
public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    private readonly string _version = "0.0.1";
    //  Major, Minor, Patch
    // <전체를 뒤엎을 변화>,<기능 수정, 기능 추가>,<버그, 내부 적 코드 보완>

    private string _nickname = "MingyuKatsu";

    private static PhotonServerManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;

        // 설정
        // 1. 버전 : 버전이 다르면, 다른 서버로 접속된다.
        PhotonNetwork.GameVersion = _version;
        // 2. 닉네임 : 게임에서 사용할 사용자의 닉네임, 중복이 가능하다.
        // 따라서 판별을 위해서는 ActorID를 사용한다.
        PhotonNetwork.NickName = _nickname;

        // 방장이 로드한 씬 게임에 참여한 다른 사용자들이 똑같이 로드할 수 있도록 동기화 해주는 옵션
        // 방장(마스터 클라이언트): 방을 만든 소유자. (방에는 하나의 마스터 클라이언트가 존재한다.)
        PhotonNetwork.AutomaticallySyncScene = true;

        // 설정값들을 이용해 서버 접속 시도
        // 정확히는 네임 서버로 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnected()
    {
        Debug.Log("네임 서버 접속 완료");
        Debug.Log(PhotonNetwork.CloudRegion);
    }

    // 포톤 마스터 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        // PhotonNetwork.JoinLobby();
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    // 로비에 접속하면 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");
        Debug.Log(PhotonNetwork.InLobby);

        // 룸 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // 룸에 입장할 수 있는 최대 접속자 수
        roomOptions.IsOpen = true;  // 룸의 오픈 여부
        roomOptions.IsVisible = true;  // 로비에서 룸 목록에 노출시킬지 여부

        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
    }

    // "내가" 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("룸 입장에 성공했습니다.");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (PhotonNetwork.IsMasterClient)
        {
            // PhotonNetwork.LoadLevel("Battle");
        }

        //// 룸에 접속한 사용자 정보
        //Dictionary<int, Photon.Realtime.Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;
        //foreach (KeyValuePair<int, Photon.Realtime.Player> player in roomPlayers)
        //{
        //    Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
        //}
    }


    // 랜덤 룸 입장에 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"룸 입장에 실패했습니다. {returnCode}:{message}");

        // 룸 생성
        // PhotonNetwork.CreateRoom("Test Room", roomOptions);
        // 룸 입장 또는 생성
        // PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
    }

    // 룸 생성에 실패하면 호출되는 콜백 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"룸 생성에 실패했습니다. {returnCode}:{message}");
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public async override void OnCreatedRoom()
    {
        Debug.Log("룸 생성에 성공했습니다.");
        // 생성된 룸 이름 확인
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        await TestPoolManager.Instance.InitializeAsync();
    }
}
