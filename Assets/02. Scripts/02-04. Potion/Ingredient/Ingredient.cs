using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Ingredient : MonoBehaviour
{
    private IngredientData _data;
    public IngredientData Data => _data;

    private MeshFilter _meshFilter;
    private PhotonView _photonView;

    [Foldout("Project")]
    [SerializeField]
    private List<MeshOnTID> _meshList = new List<MeshOnTID>();
    private Dictionary<int, Mesh> _meshDict;

    private void Awake()
    {
        InitIngredient();
    }

    private void InitIngredient()
    {
        _meshDict = new Dictionary<int, Mesh>();
        foreach (var meshInfo in _meshList)
        {
            _meshDict.Add(meshInfo.TID, meshInfo.Mesh);
        }

        _meshFilter = GetComponent<MeshFilter>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitIngredientData(int TID)
    {
        _data = DataTable.Instance.GetIngredientData(TID);
        _meshFilter.mesh = _meshDict[TID];
        _photonView.RPC(nameof(RPC_InitIngredientData), RpcTarget.Others, TID);
    }

    [PunRPC]
    public void RPC_InitIngredientData(int TID)
    {
        _data = DataTable.Instance.GetIngredientData(TID);
        _meshFilter.mesh = _meshDict[TID];
    }
}
