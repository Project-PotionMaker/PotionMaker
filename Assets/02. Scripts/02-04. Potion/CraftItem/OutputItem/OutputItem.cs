//using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[Serializable]
public class ColorOnType
{
    public EOutputType OuputType;
    public GameObject TypeObject;
    public Renderer ColorChangeRenderer;
}

public class OutputItem : MonoBehaviour, IItem
{
    private EInputType _currentInputType;
    public EInputType CurrentInputType => _currentInputType;

    private OutputData _outputData;
    public OutputData OutputData => _outputData;
    //private PhotonView _photonView;

    private MaterialPropertyBlock _materialPropertyBlock;

    [Foldout("Project")]
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

        _materialPropertyBlock = new MaterialPropertyBlock();
        //_photonView = GetComponent<PhotonView>();
    }

    public void InitOutputData(EInputType newInputType, int TID)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    throw new InvalidOperationException("Output 객체 생성 후 내부 데이터 초기화는 Master만 가능합니다.");
        //}

        _currentInputType = newInputType;
        _outputData = DataTable.Instance.GetOutputData(TID);

        foreach (var objectInfo in _colorOnTypeList)
        {
            objectInfo.TypeObject.SetActive(false);
            if (objectInfo.OuputType == _outputData.OutputType)
            {
                if (ColorUtility.TryParseHtmlString(_outputData.ColorCode, out Color parsedColor))
                {
                    _materialPropertyBlock.SetColor("_BaseColor", parsedColor);
                }
                else
                {
                    _materialPropertyBlock.SetColor("_BaseColor", Color.white);
                }

                objectInfo.ColorChangeRenderer.SetPropertyBlock(_materialPropertyBlock);
                objectInfo.TypeObject.SetActive(true);
            }
        }
        //_photonView.RPC(nameof(RPC_InitOutputData), RpcTarget.Others, newInputType, TID);
    }

    //[PunRPC]
    public void RPC_InitOutputData(EInputType newInputType, int TID)
    {
        _currentInputType = newInputType;
        _outputData = DataTable.Instance.GetOutputData(TID);

        foreach (var objectInfo in _colorOnTypeList)
        {
            objectInfo.TypeObject.SetActive(false);
            if (objectInfo.OuputType == _outputData.OutputType)
            {
                if(ColorUtility.TryParseHtmlString(_outputData.ColorCode, out Color parsedColor))
                {
                    _materialPropertyBlock.SetColor("_BaseColor", parsedColor);
                }
                else
                {
                    _materialPropertyBlock.SetColor("_BaseColor", Color.white);
                }

                objectInfo.ColorChangeRenderer.SetPropertyBlock(_materialPropertyBlock);
                objectInfo.TypeObject.SetActive(true);
            }
        }
    }

    public EInputType GetInputType()
    {
        return EInputType.Output;
    }

    public int GetTID()
    {
        return _outputData.TID;
    }
}
