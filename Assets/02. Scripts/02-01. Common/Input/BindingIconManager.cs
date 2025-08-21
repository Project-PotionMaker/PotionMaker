using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BindingIconManager : MonoBehaviourSingleton<BindingIconManager>
{
    private readonly Dictionary<EControllerType, Dictionary<string, BindingInfo>> _bindingInfoDict
        = new Dictionary<EControllerType, Dictionary<string, BindingInfo>>();

    private Dictionary<InputAction, Dictionary<EBindingType, Sprite>> _bindingSpriteDict
        = new Dictionary<InputAction, Dictionary<EBindingType, Sprite>>();

    private EBindingType _currentBindingType = EBindingType.KeyboardMain;

    public event Action OnBindingInfoChanged;

    private void Start()
    {
        ImageManager.Instance.OnInitialized += Initialize;

        InputManager.Instance.OnAnyKey += ChangeCurrentControl;

        InputMappingManager.Instance.OnRebindComplete += UpdateBindingSprite;
        InputMappingManager.Instance.OnBindingReset += UpdateAllBindingSprites;
    }

    private void OnDestroy()
    {
        if (Global.Instance != null)
        {
            Global.Instance.OnDataLoaded -= Initialize;
        }

        if (InputMappingManager.Instance != null)
        {
            InputMappingManager.Instance.OnRebindComplete -= UpdateBindingSprite;
            InputMappingManager.Instance.OnBindingReset -= UpdateAllBindingSprites;
        }
    }

    private void Initialize()
    {
        ReadOnlyList<KeyboardMouseData> keyboardMouseDataList = DataTable.Instance.GetKeyboardMouseDataList();
        _bindingInfoDict[EControllerType.Keyboard] = new Dictionary<string, BindingInfo>();
        foreach (KeyboardMouseData keyboardMouseData in keyboardMouseDataList)
        {
            _bindingInfoDict[EControllerType.Keyboard][keyboardMouseData.UnityInputPath] = new BindingInfo
            {
                DataType = typeof(KeyboardMouseData),
                TID = keyboardMouseData.TID
            };
        }

        ReadOnlyList<PlayStation5Data> playstationDataList = DataTable.Instance.GetPlayStation5DataList();
        _bindingInfoDict[EControllerType.PlayStation] = new Dictionary<string, BindingInfo>();
        foreach (PlayStation5Data playstationData in playstationDataList)
        {
            _bindingInfoDict[EControllerType.PlayStation][playstationData.UnityInputPath] = new BindingInfo
            {
                DataType = typeof(PlayStation5Data),
                TID = playstationData.TID
            };
        }

        ReadOnlyList<XboxData> xboxDataList = DataTable.Instance.GetXboxDataList();
        _bindingInfoDict[EControllerType.Xbox] = new Dictionary<string, BindingInfo>();
        foreach (XboxData xboxData in xboxDataList)
        {
            _bindingInfoDict[EControllerType.Xbox][xboxData.UnityInputPath] = new BindingInfo
            {
                DataType = typeof(XboxData),
                TID = xboxData.TID
            };
        }

        UpdateAllBindingSprites();
    }

    private void UpdateAllBindingSprites()
    {
        _bindingSpriteDict.Clear();
        InputActionAsset inputActionAsset = InputMappingManager.Instance.InputActions;

        if (inputActionAsset == null)
        {
            Debug.LogError("InputActionAsset을 찾을 수 없습니다.");
            return;
        }

        foreach (InputAction action in inputActionAsset)
        {
            _bindingSpriteDict[action] = new Dictionary<EBindingType, Sprite>();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                EBindingType bindingType = (EBindingType)i;
                string path = action.bindings[i].effectivePath;
                Sprite sprite = GetSpriteForPath(path);
                _bindingSpriteDict[action][bindingType] = sprite;
            }
        }

        OnBindingInfoChanged?.Invoke();
    }

    private void UpdateBindingSprite(InputAction action, EBindingType bindingType)
    {
        if (action == null) return;

        if (!_bindingSpriteDict.ContainsKey(action))
        {
            _bindingSpriteDict[action] = new Dictionary<EBindingType, Sprite>();
        }

        int bindingIndex = (int)bindingType;
        string path = action.bindings[bindingIndex].effectivePath;
        Sprite sprite = GetSpriteForPath(path);

        _bindingSpriteDict[action][bindingType] = sprite;

        OnBindingInfoChanged?.Invoke();
    }

    public Sprite GetCurrentInputSprite(InputAction inputAction)
    {
        if (inputAction == null)
        {
            return null;
        }

        if (_bindingSpriteDict.TryGetValue(inputAction, out var bindings) &&
            bindings.TryGetValue(_currentBindingType, out Sprite sprite))
        {
            return sprite;
        }

        return null;
    }

    public Sprite GetSpriteForPath(string inputPath)
    {
        if (string.IsNullOrEmpty(inputPath))
        {
            return null;
        }

        EControllerType controllerType = GetCurrentControllerType();
        if (inputPath.StartsWith("<Keyboard>") || inputPath.StartsWith("<Mouse>"))
        {
            controllerType = EControllerType.Keyboard;
        }

        if (_bindingInfoDict.TryGetValue(controllerType, out var deviceDict))
        {
            if (deviceDict.TryGetValue(inputPath, out BindingInfo info))
            {
                return ImageManager.Instance.GetImage(info.DataType, info.TID);
            }
        }

        return null;
    }

    private EControllerType GetCurrentControllerType()
    {
        if (Gamepad.current == null)
        {
            return EControllerType.Keyboard;
        }

        string deviceName = Gamepad.current.name.ToLower();

        if (deviceName.Contains("dualsense") || deviceName.Contains("dualshock"))
        {
            return EControllerType.PlayStation;
        }
        if (deviceName.Contains("xbox") || deviceName.Contains("xinput"))
        {
            return EControllerType.Xbox;
        }
        
        return EControllerType.Xbox;
    }

    private void ChangeCurrentControl(PlayerInput input)
    {
        EBindingType previousBindingType = _currentBindingType;
        if (input.currentControlScheme == "Gamepad")
        {
            _currentBindingType = EBindingType.GamepadMain;
        }
        else
        {
            _currentBindingType = EBindingType.KeyboardMain;
        }

        if (previousBindingType != _currentBindingType)
        {
            OnBindingInfoChanged?.Invoke();
        }
    }
}
