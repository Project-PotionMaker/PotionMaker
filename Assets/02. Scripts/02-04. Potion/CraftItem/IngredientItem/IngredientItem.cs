using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[Serializable]
public class IngredientAppearance
{
    public int TID;
    public Mesh IngredientMesh;
    public Material LiquidMaterial;
}

public class IngredientItem : MonoBehaviour, IItem
{
    private IngredientData _data;
    public IngredientData Data => _data;

    private MeshFilter _ingredientMeshFilter;
    private Renderer _ingredientRenderer;
    private PhotonView _photonView;

    [Foldout("Project")]
    [SerializeField]
    private List<IngredientAppearance> _ingeredientAppearanceList = new List<IngredientAppearance>();
    private Dictionary<int, IngredientAppearance> _ingeredientAppearanceDict;

    private void Awake()
    {
        InitIngredient();
    }

    private void InitIngredient()
    {
        _ingeredientAppearanceDict = new Dictionary<int, IngredientAppearance>();
        foreach (var ingredientAppearance in _ingeredientAppearanceList)
        {
            _ingeredientAppearanceDict.Add(ingredientAppearance.TID, ingredientAppearance);
        }

        _ingredientMeshFilter = GetComponent<MeshFilter>();
        _ingredientRenderer = GetComponent<Renderer>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitIngredientData(int TID)
    {
        _data = DataTable.Instance.GetIngredientData(TID);
        _ingredientMeshFilter.mesh = _ingeredientAppearanceDict[TID].IngredientMesh;
        _photonView.RPC(nameof(RPC_InitIngredientData), RpcTarget.Others, TID);
    }

    [PunRPC]
    public void RPC_InitIngredientData(int TID)
    {
        _data = DataTable.Instance.GetIngredientData(TID);
        _ingredientMeshFilter.mesh = _ingeredientAppearanceDict[TID].IngredientMesh;
    }

    private void InitIngredientMaterial()
    {
        
    }

    public EInputType GetInputType()
    {
        return EInputType.Ingredient;
    }

    public int GetTID()
    {
        return _data.TID;
    }
}
