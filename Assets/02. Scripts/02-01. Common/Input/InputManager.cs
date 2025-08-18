using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    private UnityEngine.InputSystem.PlayerInput _playerInput;

    private string _actionMapPlayer = "Player";
    public string ActionMapPlayer => _actionMapPlayer;

    private string _actionMapUI = "UI";
    public string ActionMapUI => _actionMapUI;

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
        _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
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
        _playerInput.actions.FindActionMap(mapToDisable, true)?.Disable();
        _playerInput.SwitchCurrentActionMap(mapToEnable);
        OnChangeInputMode?.Invoke();
    }

    // Player
    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            OnInteractChanged?.Invoke(true);
            OnAnyKey?.Invoke(_playerInput);
        }
        else
        {
            OnInteractChanged?.Invoke(false);
        }
    }

    private void OnPickup()
    {
        OnPickupEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnPing()
    {
        OnPingEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnReady()
    {
        OnReadyEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnOption()
    {
        OnOptionEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnSetting()
    {
        OnSettingEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    // UI
    private void OnPoint()
    {
        OnPointEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnNavigate()
    {
        OnNavigateEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnSubmit()
    {
        OnSubmitEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }

    private void OnCancel()
    {
        OnCancelEvent?.Invoke();
        OnAnyKey?.Invoke(_playerInput);
    }
}
