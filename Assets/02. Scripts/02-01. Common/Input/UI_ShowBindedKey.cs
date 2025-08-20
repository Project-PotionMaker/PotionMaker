using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_ShowBindedKey : MonoBehaviour
{

    [Header("Binding")]
    [SerializeField]
    private InputActionReference _actionRef;

    [Header("UI")]
    [SerializeField]
    private Image _bindImage;

    private void OnEnable()
    {
        UpdateCurrentKeyDisplay(InputManager.Instance.PlayerInput);
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

        bindingIndex = isGamePad ? (int)EBindingType.GamepadMain : (int)EBindingType.KeyboardMain;

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
