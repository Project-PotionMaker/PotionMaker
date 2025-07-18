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

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitIngredientData(IngredientData data, Mesh ingredientMesh)
    {
        _photonView.RPC(nameof(RPC_InitIngredientData), RpcTarget.All, data, ingredientMesh);
    }

    [PunRPC]
    public void RPC_InitIngredientData(IngredientData data, Mesh ingredientMesh)
    {
        _data = data;
        _meshFilter.mesh = ingredientMesh;
    }
}
