using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Mirror;
using UnityEngine.UI;

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
    private RawImage _playerImage;

    [SerializeField]
    private string _hostWaitingForPlayerDescription;
    [SerializeField]
    private string _hostReadyToPlayDescription;
    [SerializeField]
    private string _clientSHowHostDescription;
    [SerializeField]
    private string _playerWaitingForReadyDescription;

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

    public void ClearSlot()
    {
        // 기존 이벤트 구독 해제
        if (_currentRoomPlayer != null)
        {
            _currentRoomPlayer.OnClientReadyStateChanged -= RefreshReady;
        }
        
        _currentRoomPlayer = null;
        SetStatePanel(ERoomPlayerState.Offline);
    }

    public void InitPlayerInfoSlot(RoomPlayer player)
    {
        Debug.Log($"UI_PlayerInfoSlot: Initializing slot for player {player.PlayerName} (Mirror Index: {player.index}, UI Slot: {player.slotNumber})");
        
        // 기존 이벤트 구독 해제
        if (_currentRoomPlayer != null)
        {
            _currentRoomPlayer.OnClientReadyStateChanged -= RefreshReady;
        }
        
        _currentRoomPlayer = player;
        _playerNameTextList.ForEach(textUI => textUI.text = player.PlayerName);

        _onlineDescriptionTextUI.text = _playerWaitingForReadyDescription;

        if (_currentRoomPlayer.slotNumber == 0) // UI 슬롯 0번이 호스트
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
                _onlineDescriptionTextUI.text = _clientSHowHostDescription;
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

        // 이벤트 구독
        player.OnClientReadyStateChanged += RefreshReady;
    }

    public void Refresh()
    {
        if(_currentRoomPlayer == null)
        {
            SetStatePanel(ERoomPlayerState.Offline);
            return;
        }

        Debug.Log($"UI_PlayerInfoSlot: Refreshing slot for player {_currentRoomPlayer.PlayerName}");

        // 이름 업데이트
        _playerNameTextList.ForEach(textUI => textUI.text = _currentRoomPlayer.PlayerName);

        if (_currentRoomPlayer.slotNumber == 0) // UI 슬롯 0번이 호스트
        {
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
            else
            {
                _onlineDescriptionTextUI.text = _clientSHowHostDescription;
            }
        }
        else
        {
            _onlineDescriptionTextUI.text = _playerWaitingForReadyDescription;
        }
    }

    public void RefreshReady()
    {
        if (_currentRoomPlayer == null) return;
        
        Debug.Log($"UI_PlayerInfoSlot: Refreshing ready state for player {_currentRoomPlayer.PlayerName} - Ready: {_currentRoomPlayer.readyToBegin}");
        
        if (_currentRoomPlayer.readyToBegin)
        {
            SetStatePanel(ERoomPlayerState.Ready);
        }
        else
        {
            SetStatePanel(ERoomPlayerState.Online);
        }

        if (_currentRoomPlayer.slotNumber == 0 && _currentRoomPlayer.isLocalPlayer && NetworkServer.active)
        {
            RefreshHostDescription();
        }
    }

    public void RefreshHostDescription()
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

    public void SetStatePanel(ERoomPlayerState state)
    {
        Debug.Log($"UI_PlayerInfoSlot: Setting state to {state}");
        
        if(state == ERoomPlayerState.Offline)
        {
            _onlinePanel.SetActive(false);
            _readyPanel.SetActive(false);
            _offlinePanel.SetActive(true);
            _playerImage.DOKill();
            _playerImage.DOColor(new Color(1f, 1f, 1f, 0f), 0.3f);
        }
        else if(state == ERoomPlayerState.Online)
        {
            _onlinePanel.SetActive(true);
            _readyPanel.SetActive(false);
            _offlinePanel.SetActive(false);
            _playerImage.DOKill();
            _playerImage.DOColor(Color.white, 0.3f);
        }
        else
        {
            _onlinePanel.SetActive(false);
            _readyPanel.SetActive(true);
            _offlinePanel.SetActive(false);
            _playerImage.DOKill();
            _playerImage.DOColor(Color.white, 0.3f);
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
        Debug.Log("UI_PlayerInfoSlot: Client disconnected");
        SetStatePanel(ERoomPlayerState.Offline);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (_currentRoomPlayer != null)
        {
            _currentRoomPlayer.OnClientReadyStateChanged -= RefreshReady;
        }
    }
}
