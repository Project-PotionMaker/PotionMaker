using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_TestEnterRoom : MonoBehaviourPunCallbacks
{
    public GameObject RoomInstantiatePanel;
    public GameObject RoomEnterPanel;
    public TMP_InputField roomIDInputField;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RoomInstantiatePanel.SetActive(false);
            RoomEnterPanel.SetActive(false);
        }
    }

    public void OnClickInstantiateRoomPanelOn()
    {
        RoomInstantiatePanel.SetActive(true);
    }

    public void OnClickEnterRoomPanelOn()
    {
        RoomEnterPanel.SetActive(true);
    }

    public void OnClickEnterRoomButton()
    {
        // Room 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true;      // 룸 입장 가능 여부
        roomOptions.IsVisible = true;   // 로비 (채널) 룸 목록에 노출시킬지 여부

        PhotonNetwork.JoinOrCreateRoom(roomIDInputField.text, roomOptions, TypedLobby.Default);
    }

    public void OnClickInstantiateRoom(int index)
    {
        // Room 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsOpen = true;      // 룸 입장 가능 여부
        roomOptions.IsVisible = true;   // 로비 (채널) 룸 목록에 노출시킬지 여부

        PhotonNetwork.JoinOrCreateRoom(index.ToString(), roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(int.Parse(PhotonNetwork.CurrentRoom.Name));
        }
    }
}
