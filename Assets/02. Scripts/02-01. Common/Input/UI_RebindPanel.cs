using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RebindPanel : MonoBehaviour
{
    [Header("키 타입 버튼")]
    [SerializeField]
    private Button _keyboardButton;
    [SerializeField]
    private Button _gamepadButton;

    [Header("MainKey")]
    [SerializeField]
    private List<UI_RebindAction> _mainkeyList;
    [SerializeField]
    private List<UI_RebindAction> _subkeyList;

    private void Start()
    {
        _keyboardButton.onClick.AddListener( () => ChangeKeyTypeSetting(EBindingType.KeyboardMain));
        _gamepadButton.onClick.AddListener( () => ChangeKeyTypeSetting(EBindingType.GamepadMain));
    }

    private void ChangeKeyTypeSetting(EBindingType mainBindingType)
    {
        foreach (UI_RebindAction mainkey in _mainkeyList)
        {
            mainkey.BindingType = mainBindingType;
            mainkey.Initialize();
        }

        foreach (UI_RebindAction subkey in _subkeyList)
        {
            subkey.BindingType = mainBindingType + 1;
            subkey.Initialize();
        }
    }
}
