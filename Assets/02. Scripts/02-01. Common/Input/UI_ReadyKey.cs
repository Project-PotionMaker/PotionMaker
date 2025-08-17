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
            Debug.Log($"[UI_ReadyKey] {name} (id={GetInstanceID()}) " +
              $"active={gameObject.activeInHierarchy}, enabled={enabled}, " +
              $"bindImageNull={_bindImage == null}");
            _bindImage.sprite = icon;
            _bindImage.gameObject.SetActive(true);
        }
        else
        {
            _bindImage.gameObject.SetActive(false);
        }
    }
}
