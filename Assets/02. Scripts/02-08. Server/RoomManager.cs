using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviourSingleton<RoomManager>, IMatchmakingCallbacks, IInRoomCallbacks
{
    private EAddressableKeys _playerAddressableKey = EAddressableKeys.Prefab_Player;
    private GameObject _playerPrefab = null;

    private Room _room;
    public Room Room => _room;

    public event Action OnRoomDataChanged;
    public event Action<string> OnPlayerEntered;
    public event Action<string> OnPlayerExited;

    private bool _initialized = false;

    protected override void Awake()
    {
        base.Awake();

        SetPlayerPrefab();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnJoinedRoom()
    {
        Init();
    }

    private void Init()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        // 1. 플레이어 생성
        GeneratePlayer();

        // 2. 룸 설정
        SetRoom();

        OnRoomDataChanged?.Invoke();
    }

    // TODO : 나중에 로딩씬에서 실행해야 합니다.(메서드 이전 필요)
    public async void SetPlayerPrefab()
    {
        DefaultPool defaultPool = PhotonNetwork.PrefabPool as DefaultPool;

        string playerAddressableKey = _playerAddressableKey.ToString();

        _playerPrefab = await AssetManager.Instance.LoadAsset<GameObject>(playerAddressableKey);

        defaultPool.ResourceCache.Add(playerAddressableKey, _playerPrefab);
    }

    private void GeneratePlayer()
    {
        // 방에 입장 완료가 되면 플레이어를 생성한다.
        // 포톤에서는 게임 오브젝트 생성 후 포톤 서버에 등록까지해야 한다.

        string playerAddressableKey = _playerAddressableKey.ToString();
        PhotonNetwork.Instantiate(playerAddressableKey, Vector3.zero, Quaternion.identity);
    }

    private void SetRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        Debug.Log(_room.Name);
        Debug.Log(_room.PlayerCount);
        Debug.Log(_room.MaxPlayers);
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerEntered?.Invoke(newPlayer.NickName + "_" + newPlayer.ActorNumber);
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerExited?.Invoke(otherPlayer.NickName + "_" + otherPlayer.ActorNumber);
    }


    public void OnCreatedRoom() { }
    public void OnCreateRoomFailed(short returnCode, string message) { }
    public void OnFriendListUpdate(List<FriendInfo> friendList) { }
    public void OnJoinRandomFailed(short returnCode, string message) { }
    public void OnJoinRoomFailed(short returnCode, string message) { }
    public void OnLeftRoom() { }
    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) { }
    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) { }
}
