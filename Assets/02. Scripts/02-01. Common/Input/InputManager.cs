using Mirror.Examples.BilliardsPredicted;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    private UnityEngine.InputSystem.PlayerInput _playerInput;

    // Player
    public event Action<bool> OnInteractChanged;
    public event Action OnPickupEvent;
    public event Action OnPingEvent;
    public event Action OnReadyEvent;
    public event Action OnOptionEvent;
    public event Action OnSettingEvent;

    // UI
    public event Action<Vector2> OnNavigateEvent;
    public event Action OnSelectEvent;
    public event Action OnCloseEvent;

    protected override void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    public void ChangeToUIInput()
    {
        _playerInput.SwitchCurrentActionMap("UI");
    }

    public void ChangeToPlayerInput()
    {
        _playerInput.SwitchCurrentActionMap("Player");
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

    private void OnNavigate(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        OnNavigateEvent?.Invoke(inputVector);
    }

    private void OnSelect()
    {
        OnSelectEvent?.Invoke();
    }

    private void OnClose()
    {
        OnCloseEvent?.Invoke();
    }
}
