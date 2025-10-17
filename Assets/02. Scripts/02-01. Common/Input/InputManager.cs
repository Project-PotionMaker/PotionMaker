using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

    private const string ACRION_MAP_PlAYER = "Player";
    public string ActionMapPlayer => ACRION_MAP_PlAYER;

    private const string ACTION_MAP_UI = "UI";
    public string ActionMapUI => ACTION_MAP_UI;

    private const string POINT = "Point";
    private const string CLICK = "Click";
    private const string READY = "Ready";

    // Player
    public event Action<bool> OnInteractChanged;
    public event Action OnPickupEvent;
    public event Action OnPingEvent;
    public event Action OnReadyEvent;
    public event Action OnOptionEvent;
    public event Action OnSettingEvent;

    // UI
    public event Action OnPointEvent;
    public event Action OnNavigateEvent;
    public event Action OnSubmitEvent;
    public event Action OnCancelEvent;

    public event Action<PlayerInput> OnAnyKey;

    public event Action OnChangeInputMode;

    protected override void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<PlayerInput>();
    }

    public void ChangeToUIInput()
    {
        SwitchMap(ACRION_MAP_PlAYER, ACTION_MAP_UI);
    }

    public void ChangeToPlayerInput()
    {
        SwitchMap(ACTION_MAP_UI, ACRION_MAP_PlAYER);
    }

    private void SwitchMap(string mapToDisable, string mapToEnable)
    {
        var fromMap = _playerInput.actions.FindActionMap(mapToDisable, true);
        fromMap.Disable();
        _playerInput.SwitchCurrentActionMap(mapToEnable);
        // UI 모드에서 Player 모드로 전환될 때, UI 클릭을 계속 허용하기 위해
        // 비활성화된 UI 액션 맵의 마우스 관련 액션을 다시 활성화합니다.
        if (mapToDisable == ACTION_MAP_UI)
        {
            fromMap.FindAction(POINT, false)?.Enable();
            fromMap.FindAction(CLICK, false)?.Enable();
        }
        else
        {
            fromMap.FindAction(READY,false)?.Enable();
        }

        OnChangeInputMode?.Invoke();
    }

    // Player
    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            OnInteractChanged?.Invoke(true);
        }
        else
        {
            OnInteractChanged?.Invoke(false);
        }
    }

    private void OnPickup()
    {
        OnPickupEvent?.Invoke();
    }

    private void OnPing()
    {
        OnPingEvent?.Invoke();
    }

    private void OnReady()
    {
        OnReadyEvent?.Invoke();
    }

    private void OnOption()
    {
        OnOptionEvent?.Invoke();
    }

    private void OnSetting()
    {
        OnSettingEvent?.Invoke();
    }

    // UI
    private void OnPoint()
    {
        OnPointEvent?.Invoke();
    }

    private void OnNavigate()
    {
        OnNavigateEvent?.Invoke();
    }

    private void OnSubmit()
    {
        OnSubmitEvent?.Invoke();
    }

    private void OnCancel()
    {
        OnCancelEvent?.Invoke();
    }

    private void OnControlsChanged(PlayerInput input)
    {
        OnAnyKey?.Invoke(input);
    }
}
