using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Mirror;

public enum ERoomPlayerState
{
    Offline,
    Online,
    Ready
}

public class UI_PlayerInfoSlot : MonoBehaviour
{
    [SerializeField]
    private float _resizeSpeed = 100f;

    [SerializeField]
    private List<TextMeshProUGUI> _playerNameTextList;
    [SerializeField]
    private TextMeshProUGUI _readyText;
    [SerializeField]
    private TextMeshProUGUI _onlineDescriptionText;
    [SerializeField]
    private TextMeshProUGUI _offlineText;

    [SerializeField]
    private GameObject _onlinePanel;
    [SerializeField]
    private GameObject _readyPanel;
    [SerializeField]
    private GameObject _offlinePanel;

    [SerializeField]
    private string _hostWaitingForPlayerDescription;
    [SerializeField]
    private string _hostReadyToPlayDescription;
    [SerializeField]
    private string _PlayerWaitingForReadyDescription;

    private Dictionary<ERoomPlayerState, float> _stateHeightDict;

    private RoomPlayer _currentRoomPlayer;
    public RoomPlayer CurrentRoomPlayer => _currentRoomPlayer;

    private RectTransform _slotRectTransform;

    public void Awake()
    {
        _slotRectTransform = GetComponent<RectTransform>();
        _stateHeightDict = new Dictionary<ERoomPlayerState, float>
        {
            { ERoomPlayerState.Offline, 400 },
            { ERoomPlayerState.Ready, 500 },
            { ERoomPlayerState.Online, 600 }
        };
        SetStatePanel(ERoomPlayerState.Offline);
    }

    public void InitPlayerInfoSlot(RoomPlayer player)
    {
        _currentRoomPlayer = player;
        _playerNameTextList.ForEach(textUI => textUI.text = player.PlayerName);

        _onlineDescriptionText.text = _PlayerWaitingForReadyDescription;
        // 호스트면
        if (player.isLocalPlayer && NetworkServer.active)
        {
            _onlineDescriptionText.text = _hostReadyToPlayDescription;
        }


        if (player.readyToBegin)
        {
            SetStatePanel(ERoomPlayerState.Ready);
        }
        else
        {
            SetStatePanel(ERoomPlayerState.Online);
        }

        player.OnClientReadyStateChanged += Refresh;
    }

    public void Refresh()
    {
        if(_currentRoomPlayer == null)
        {
            SetStatePanel(ERoomPlayerState.Offline);
        }

        if (_currentRoomPlayer.readyToBegin)
        {
            SetStatePanel(ERoomPlayerState.Ready);
        }
        else
        {
            SetStatePanel(ERoomPlayerState.Online);
        }
    }

    public void SetStatePanel(ERoomPlayerState state)
    {
        if(state == ERoomPlayerState.Offline)
        {
            _onlinePanel.SetActive(false);
            _readyPanel.SetActive(false);
            _offlinePanel.SetActive(true);
        }
        else if(state == ERoomPlayerState.Online)
        {
            _onlinePanel.SetActive(true);
            _readyPanel.SetActive(false);
            _offlinePanel.SetActive(false);
        }
        else
        {
            _onlinePanel.SetActive(false);
            _readyPanel.SetActive(true);
            _offlinePanel.SetActive(false);
        }

        float currentHeight = _slotRectTransform.rect.height;
        float distance = Mathf.Abs(_stateHeightDict[state] - currentHeight);
        float duration = distance / _resizeSpeed;

        Vector2 targetSize = new Vector2(_slotRectTransform.sizeDelta.x, _stateHeightDict[state]);
        _slotRectTransform.DOSizeDelta(targetSize, duration).SetEase(Ease.OutCubic);
    }
}
