using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[Serializable]
public class MeshOnType
{
    public EInputType EInputType;
    public Mesh Mesh;
}

public class Output : MonoBehaviour
{
    private EInputType _currentInputType;
    public EInputType CurrentInputType => _currentInputType;

    private OutputData _outputData;
    public OutputData OutputData => _outputData;

    private MeshFilter _meshFilter;
    private PhotonView _photonView;

    [Foldout("Project")]
    [SerializeField]
    private List<MeshOnType> _meshList = new List<MeshOnType>();
    private Dictionary<EInputType, Mesh> _meshDict;

    private void Awake()
    {
        InitOutput();
    }

    private void InitOutput()
    {
        _meshDict = new Dictionary<EInputType, Mesh>();
        foreach (var meshInfo in _meshList)
        {
            _meshDict.Add(meshInfo.EInputType, meshInfo.Mesh);
        }

        _meshFilter = GetComponent<MeshFilter>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitOutputData(EInputType newInputType, int TID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Output 객체 생성 후 내부 데이터 초기화는 Master만 가능합니다.");
        }

        _currentInputType = newInputType;
        _outputData = DataTable.Instance.GetOutputData(TID);
        _meshFilter.mesh = _meshDict[newInputType];
        _photonView.RPC(nameof(RPC_InitOutputData), RpcTarget.Others, newInputType, TID);
    }

    [PunRPC]
    public void RPC_InitOutputData(EInputType newInputType, int TID)
    {
        _currentInputType = newInputType;
        _outputData = DataTable.Instance.GetOutputData(TID);
        _meshFilter.mesh = _meshDict[newInputType];
    }
}
