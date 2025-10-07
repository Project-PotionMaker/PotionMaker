using Mirror.Discovery;
using TMPro;
using UnityEngine;

public class CodeInputUI : MonoBehaviour
{
    [SerializeField] 
    private TMP_InputField _inputField;
    [SerializeField]
    private TextMeshProUGUI _warningField;

    private string _pendingRoomCode;

    public void OnJoinByCode()
    {
        string text = _inputField.text.Trim();
        _warningField.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(text) || text.Length != 5)
        {
            _warningField.text = "방 코드는 5글자의 숫자 또는 영어가 포함된 글자입니다.";
            _warningField.gameObject.SetActive(true);
            return;
        }

        _pendingRoomCode = text.ToUpper();
        if (SteamLobby.Instance != null)
        {
            SteamLobby.Instance.RequestLobbyByRoomCode(_pendingRoomCode, OnLobbySearchResult);
            return;
        }
        string address = RoomDirectory.Instance.GetAddress(_pendingRoomCode);

        if (string.IsNullOrEmpty(address))
        {
            _warningField.text = "존재하지 않는 방입니다.";
            _warningField.gameObject.SetActive(true);
            return;
        }

        MirrorNetworkManager.Instance.networkAddress = address;
        MirrorNetworkManager.Instance.StartClient();
    }

    // 콜백: 로비가 검색되었을 때 처리
    private void OnLobbySearchResult(bool found)
    {
        if (!found)
        {
            _warningField.text = "존재하지 않는 방입니다.";
            _warningField.gameObject.SetActive(true);
        }
    }
}
