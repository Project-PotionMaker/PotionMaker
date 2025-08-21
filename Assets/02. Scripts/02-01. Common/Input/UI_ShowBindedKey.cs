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
        BindingIconManager.Instance.OnBindingInfoChanged += UpdateKeyDisplay;

        UpdateKeyDisplay();
    }

    private void OnDisable()
    {
        if (BindingIconManager.Instance != null)
        {
            BindingIconManager.Instance.OnBindingInfoChanged -= UpdateKeyDisplay;
        }
    }

    private void UpdateKeyDisplay()
    {
        if (_actionRef == null || _actionRef.action == null)
        {
            return;
        }

        Sprite icon = BindingIconManager.Instance.GetCurrentInputSprite(_actionRef);

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
