using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    [SerializeField]
    private InputActionAsset _inputActions;
    private InputAction _inputAction;

    private void Start()
    {
        _inputActions.Enable();
    }
}
