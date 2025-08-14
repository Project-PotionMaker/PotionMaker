using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    public event Action<bool> OnInteractChanged;
    public event Action OnPickupEvent;
    public event Action OnPingEvent;
    public event Action OnReadyEvent;
    public event Action OnOptionEvent;

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
}
