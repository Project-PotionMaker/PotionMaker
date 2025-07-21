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

public class PotionItem : MonoBehaviour
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
        InitPotion();
    }

    private void InitPotion()
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
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Potion 객체 생성 후 내부 데이터 초기화는 Master만 가능합니다.");
        }
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
