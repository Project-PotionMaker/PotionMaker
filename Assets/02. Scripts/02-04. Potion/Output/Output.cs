using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

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
    private List<(EInputType, Mesh)> _meshList;

    private Dictionary<EInputType, Mesh> _meshDict;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _meshDict = new Dictionary<EInputType, Mesh>();
        foreach (var meshInfo in _meshList)
        {
            _meshDict.Add(meshInfo.Item1, meshInfo.Item2);
        }

        _meshFilter = GetComponent<MeshFilter>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitOutputData(EInputType newInputType, OutputData newData, string newOutputCode)
    {
        _photonView.RPC(nameof(RPC_InitOutputData), RpcTarget.All, newInputType, newData, newOutputCode);
    }

    [PunRPC]
    public void RPC_InitOutputData(EInputType newInputType, OutputData newData, string newOutputCode)
    {
        _currentInputType = newInputType;
        _outputData = newData;
        _meshFilter.mesh = _meshDict[newInputType];
    }
}
