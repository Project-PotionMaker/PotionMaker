using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[Serializable]
public class MeshOnTID
{
    public int TID;
    public Mesh Mesh;
}

public class Potion : MonoBehaviour
{
    private PotionData _potionData;
    public PotionData PotionData => _potionData;

    private MeshFilter _meshFilter;
    private PhotonView _photonView;

    [Foldout("Project")]
    [SerializeField]
    private List<MeshOnTID> _meshList = new List<MeshOnTID>();
    private Dictionary<int, Mesh> _meshDict;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _meshDict = new Dictionary<int, Mesh>();
        foreach (var meshInfo in _meshList)
        {
            _meshDict.Add(meshInfo.TID, meshInfo.Mesh);
        }

        _meshFilter = GetComponent<MeshFilter>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitPotionData(int TID)
    {
        _potionData = DataTable.Instance.GetPotionData(TID);
        _meshFilter.mesh = _meshDict[TID];
        _photonView.RPC(nameof(RPC_InitPotionData), RpcTarget.Others, TID);
    }

    [PunRPC]
    public void RPC_InitPotionData(int TID)
    {
        _potionData = DataTable.Instance.GetPotionData(TID);
        _meshFilter.mesh = _meshDict[TID];
    }
}
