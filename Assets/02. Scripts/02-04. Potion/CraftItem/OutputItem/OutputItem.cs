using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorOnType
{
    public EOutputType OuputType;
    public GameObject TypeObject;
    public Renderer ColorChangeRenderer;
}

public class OutputItem : NetworkBehaviour, IItem
{
    [SyncVar(hook = nameof(OnOutputItemTIDUpdated))]
    private int _outputTID;
    public int OutputTID => _outputTID;

    [SyncVar]
    private EInputType _currentInputType;
    public EInputType CurrentInputType => _currentInputType;

    private OutputData _outputData;
    public OutputData OutputData => _outputData;

    private MaterialPropertyBlock _mpb;

    [SerializeField]
    private List<ColorOnType> _colorOnTypeList = new List<ColorOnType>();
    private Dictionary<EOutputType, ColorOnType> _colorOnTypeDict;

    private void Awake()
    {
        InitOutput();
    }

    private void InitOutput()
    {
        _colorOnTypeDict = new Dictionary<EOutputType, ColorOnType>();
        foreach (var objectInfo in _colorOnTypeList)
        {
            objectInfo.TypeObject.SetActive(false);
            _colorOnTypeDict.Add(objectInfo.OuputType, objectInfo);
        }
        _mpb = new MaterialPropertyBlock();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void OnOutputItemTIDUpdated(int oldValue, int newValue)
    {
        ClientUpdateOutputData();
    }

    [Server]
    public void ServerUpdateOutputData(EInputType newInputType, int TID)
    {
        _currentInputType = newInputType;
        _outputTID = TID;
        _outputData = DataTable.Instance.GetOutputData(TID);
    }

    private void ClientUpdateOutputData()
    {
        // 클라이언트에서 초기화 시 SyncVar로 받은 TID와 Type을 사용
        _outputData = DataTable.Instance.GetOutputData(_outputTID);

        // 데이터에 따라 모델 및 색상을 한 번만 설정
        if (_colorOnTypeDict.TryGetValue(_outputData.OutputType, out ColorOnType objectInfo))
        {
            if (ColorUtility.TryParseHtmlString(_outputData.ColorCode, out Color parsedColor))
            {
                _mpb.SetColor("_BaseColor", parsedColor);
            }
            else
            {
                _mpb.SetColor("_BaseColor", Color.white);
            }

            objectInfo.ColorChangeRenderer.SetPropertyBlock(_mpb);
            objectInfo.TypeObject.SetActive(true);
        }
    }

    public EInputType GetInputType()
    {
        return _currentInputType;
    }

    public int GetTID()
    {
        return _outputTID;
    }
}