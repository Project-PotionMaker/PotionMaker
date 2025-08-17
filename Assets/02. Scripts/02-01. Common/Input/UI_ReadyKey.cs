using NUnit.Framework.Internal.Commands;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class UI_ReadyKey : MonoBehaviour
{
    [Header("Binding")]
    [SerializeField]
    private InputActionReference _actionRef;

    private InputAction _inputAction;

    [Header("UI")]
    [SerializeField]
    private Image _bindImage;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_actionRef == null || _actionRef.action == null)
        {
            Debug.LogWarning("[UI_InteractKey] InputActionReference가 비었어요.");
            return;
        }
        InputManager.Instance.OnAnyKey += UpdateCurrentKeyDisplay;
    }
    private void OnDisable()
    {
        InputManager.Instance.OnAnyKey -= UpdateCurrentKeyDisplay;
    }
    private void UpdateCurrentKeyDisplay(PlayerInput playerInput)
    {
        int bindingIndex = (int)EBindingType.KeyboardMain;
        bool isGamePad = string.Equals(playerInput.currentControlScheme, "Gamepad", StringComparison.OrdinalIgnoreCase);

        if (isGamePad)
        {
            bindingIndex = (int)EBindingType.GamepadMain;
        }
        else
        {
            bindingIndex = (int)EBindingType.KeyboardMain;
        }
        string path = _actionRef.action.bindings[bindingIndex].effectivePath;

        Sprite icon = BindingIconManager.Instance.GetSpriteForPath(path);

        if (icon != null)
        {
            _bindImage.sprite = icon;
            _bindImage.gameObject.SetActive(true);
        }
        else
        {
            _bindImage.gameObject.SetActive(false);
        }
    }
}
