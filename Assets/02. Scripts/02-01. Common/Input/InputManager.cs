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

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
        Debug.Log(_moveInput);
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            OnInteractChanged?.Invoke(true);
            Debug.Log("Interact : true");
        }
        else
        {
            OnInteractChanged?.Invoke(false);
            Debug.Log("Interact : false");
        }
    }

    private void OnPickup()
    {
        OnPickupEvent?.Invoke();
        Debug.Log("Pickup");
    }

    private void OnPing()
    {
        OnPingEvent?.Invoke();
        Debug.Log("Ping");
    }

    private void OnReady()
    {
        OnReadyEvent?.Invoke();
        Debug.Log("Ready");
    }
}
