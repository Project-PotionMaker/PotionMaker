using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

    private string _actionMapPlayer = "Player";
    public string ActionMapPlayer => _actionMapPlayer;

    private string _actionMapUI = "UI";
    public string ActionMapUI => _actionMapUI;

    private const string POINT = "Point";
    private const string CLICK = "Click";

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
        SwitchMap(_actionMapPlayer, _actionMapUI);
    }

    public void ChangeToPlayerInput()
    {
        SwitchMap(_actionMapUI, _actionMapPlayer);
    }

    private void SwitchMap(string mapToDisable, string mapToEnable)
    {
        var fromMap = _playerInput.actions.FindActionMap(mapToDisable, true);
        fromMap.Disable();
        _playerInput.SwitchCurrentActionMap(mapToEnable);
        // UI 모드에서 Player 모드로 전환될 때, UI 클릭을 계속 허용하기 위해
        // 비활성화된 UI 액션 맵의 마우스 관련 액션을 다시 활성화합니다.
        if (mapToDisable == _actionMapUI)
        {
            fromMap.FindAction(POINT, false)?.Enable();
            fromMap.FindAction(CLICK, false)?.Enable();
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
