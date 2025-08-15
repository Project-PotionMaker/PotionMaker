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

    protected override void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    public void ChangeToUIInput()
    {
        _playerInput.SwitchCurrentActionMap(_actionMapUI);
    }

    public void ChangeToPlayerInput()
    {
        _playerInput.SwitchCurrentActionMap(_actionMapPlayer);
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
}
