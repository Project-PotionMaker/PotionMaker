using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkRoomPlayer
{
    //[SyncVar(hook = nameof(OnPlayerNameChanged))] // 플레이어 이름 동기화 예시
    public string playerName = "Player";

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //Debug.Log($"Local player started: {connectionToClient.connectionId}");

        //// 이 플레이어 인스턴스가 내 것일 때만 UI 활성화
        //if (isLocalPlayer)
        //{
        //    // 플레이어 이름 설정 (예시)
        //    playerName = "MyPlayer_" + Random.Range(1, 100); // 닉네임 설정
        //    CmdSetPlayerName(playerName);
        //}
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        //Debug.Log($"Client entered room: {connectionToClient.connectionId}");
        // 로비 UI에 플레이어 목록 업데이트
        // NetworkRoomManager.singleton.roomSlots 리스트에서 모든 NetworkRoomPlayer를 가져와 UI 업데이트
    }

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        //Debug.Log($"Client exited room: {connectionToClient.connectionId}");
        // 로비 UI에서 플레이어 제거
    }

    //public override void OnClientReady(bool readyState)
    //{
    //    base.OnClientReady(readyState);
    //    Debug.Log($"Client {connectionToClient.connectionId} ready state changed to: {readyState}");
    //    // UI에 준비 상태 표시 업데이트
    //    UpdateReadyStateUI();
    //}

    //// --- Command (클라이언트 -> 서버) ---
    //[Command]
    //void CmdSetPlayerName(string newName)
    //{
    //    playerName = newName; // SyncVar에 의해 모든 클라이언트로 동기화됨
    //}

    //// --- Hook (SyncVar 변경 시 호출) ---
    //void OnPlayerNameChanged(string oldName, string newName)
    //{
    //    if (playerNameText != null)
    //    {
    //        playerNameText.text = newName;
    //    }
    //}

    //void OnReadyButtonClicked()
    //{
    //    // readyToBegin 상태를 토글
    //    CmdChangeReadyState(!readyToBegin);
    //}

    //void UpdateReadyStateUI()
    //{
    //    // 준비 버튼 텍스트나 색상 변경 등
    //    if (readyButton != null)
    //    {
    //        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = readyToBegin ? "준비됨!" : "준비";
    //        readyButton.interactable = !readyToBegin; // 한번 준비하면 다시 누르지 못하도록
    //    }
    //}
}
