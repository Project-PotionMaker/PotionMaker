using Mirror;
using System;
using System.Collections;
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
    public event Action<List<int>, List<int>> OnOutputTIDUpdated;
    public event Action<bool> OnItemFocusChanged;

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

    [SerializeField]
    private GameObject _models;
    private Coroutine _visibleRoutine;

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
        if (!ReferenceEquals(_visibleRoutine, null))
        {
            StopCoroutine(_visibleRoutine);
        }
        if (gameObject.activeInHierarchy)
        {
            _visibleRoutine = StartCoroutine(Coroutine_VisibleRoutine());
        }
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

        foreach (var objectInf in _colorOnTypeList)
        {
            objectInf.TypeObject.SetActive(false);
        }
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
            ResetModel();
            objectInfo.TypeObject.SetActive(true);
        }

        OnOutputTIDUpdated?.Invoke
            (_outputData.AvailableMachineTIDList, _outputData.IngredientTIDList);
    }

    private void ResetModel()
    {
        if (_colorOnTypeList == null) return;
        foreach (var modelInfo in _colorOnTypeList)
        {
            modelInfo.TypeObject.SetActive(false);
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

    private IEnumerator Coroutine_VisibleRoutine()
    {
        _models.SetActive(false);
        yield return new WaitForSeconds(.05f);
        _models.SetActive(true);
    }

    [TargetRpc]
    public void TargetRpcSetFocus(NetworkConnection target, bool isActive)
    {
        Debug.Log(nameof(TargetRpcSetFocus));
        SetFocus(isActive);
    }

    public void SetFocus(bool isActive)
    {
        OnItemFocusChanged?.Invoke(isActive);
    }
}