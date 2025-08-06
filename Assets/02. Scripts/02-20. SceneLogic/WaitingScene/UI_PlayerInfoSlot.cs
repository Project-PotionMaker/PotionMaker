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
    private TextMeshProUGUI _readyTextUI;
    [SerializeField]
    private TextMeshProUGUI _onlineDescriptionTextUI;
    [SerializeField]
    private TextMeshProUGUI _offlineTextUI;

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
    private string _ClientSHowHostDescription;
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

        _onlineDescriptionTextUI.text = _PlayerWaitingForReadyDescription;

        if (_currentRoomPlayer.index == 0)
        {
            if (player.isLocalPlayer && NetworkServer.active)
            {
                _onlineDescriptionTextUI.text = _hostWaitingForPlayerDescription;
                if (_currentRoomPlayer.CheckPlayersReadyForHost())
                {
                    _onlineDescriptionTextUI.text = _hostReadyToPlayDescription;
                }
            }
            else
            {
                _onlineDescriptionTextUI.text = _ClientSHowHostDescription;
            }
        }

        if (player.readyToBegin)
        {
            SetStatePanel(ERoomPlayerState.Ready);
        }
        else
        {
            SetStatePanel(ERoomPlayerState.Online);
        }

        player.OnClientReadyStateChanged += RefreshReady;
    }

    public void Refresh()
    {
        if(_currentRoomPlayer == null)
        {
            SetStatePanel(ERoomPlayerState.Offline);
            return;
        }

        if (_currentRoomPlayer.isLocalPlayer && NetworkServer.active)
        {
            if (_currentRoomPlayer.CheckPlayersReadyForHost())
            {
                _onlineDescriptionTextUI.text = _hostReadyToPlayDescription;
            }
            else
            {
                _onlineDescriptionTextUI.text = _hostWaitingForPlayerDescription;
            }
        }
    }

    public void RefreshReady()
    {
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

        _slotRectTransform.DOKill();

        float currentHeight = _slotRectTransform.rect.height;
        float distance = Mathf.Abs(_stateHeightDict[state] - currentHeight);
        float duration = distance / _resizeSpeed;

        Vector2 targetSize = new Vector2(_slotRectTransform.sizeDelta.x, _stateHeightDict[state]);
        _slotRectTransform.DOSizeDelta(targetSize, duration).SetEase(Ease.OutCubic);
    }

    public void OnClientDisconnect()
    {
        SetStatePanel(ERoomPlayerState.Offline);
    }
}
