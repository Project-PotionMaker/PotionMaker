using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BindingIconManager : MonoBehaviourSingleton<BindingIconManager>
{
    private readonly Dictionary<EControllerType, Dictionary<string, BindingInfo>> _bindingInfoDict
        = new Dictionary<EControllerType, Dictionary<string, BindingInfo>>();

    private void Start()
    {
        Global.Instance.OnDataLoaded += Initialize;
    }

    private void OnDestroy()
    {
        if (Global.Instance != null)
        {
            Global.Instance.OnDataLoaded -= Initialize;
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
    }

    public Sprite GetSpriteForPath(string inputPath)
    {
        EControllerType controllerType = GetCurrentControllerType();
        if (inputPath.StartsWith("<Keyboard>"))
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
        
        return EControllerType.Generic;
    }
}
