using System.Collections.Generic;
using UnityEngine;

public class BindingIconManager : MonoBehaviourSingleton<BindingIconManager>
{
    private readonly Dictionary<string, BindingInfo> _bindingInfoDict = new Dictionary<string, BindingInfo>();

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
        foreach (KeyboardMouseData keyboardMouseData in keyboardMouseDataList)
        {
            _bindingInfoDict[keyboardMouseData.UnityInputPath] = new BindingInfo
            {
                DataType = typeof(KeyboardMouseData),
                TID = keyboardMouseData.TID
            };
        }

        ReadOnlyList<PlayStation5Data> playstationDataList = DataTable.Instance.GetPlayStation5DataList();
        foreach (PlayStation5Data playstationData in playstationDataList)
        {
            _bindingInfoDict[playstationData.UnityInputPath] = new BindingInfo
            {
                DataType = typeof(PlayStation5Data),
                TID = playstationData.TID
            };
        }

        ReadOnlyList<XboxData> xboxDataList = DataTable.Instance.GetXboxDataList();
        foreach (XboxData xboxData in xboxDataList)
        {
            _bindingInfoDict[xboxData.UnityInputPath] = new BindingInfo
            {
                DataType = typeof(XboxData),
                TID = xboxData.TID
            };
        }
    }

    public Sprite GetSpriteForPath(string inputPath)
    {
        if (_bindingInfoDict.TryGetValue(inputPath, out BindingInfo info))
        {
            return ImageManager.Instance.GetImage(info.DataType, info.TID);
        }

        return null;
    }
}
