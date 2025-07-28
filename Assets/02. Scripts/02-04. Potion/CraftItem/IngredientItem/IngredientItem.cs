using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class IngredientItem : MonoBehaviour, IItem
{
    private IngredientData _data;
    public IngredientData Data => _data;

    private MeshFilter _ingredientMeshFilter;
    private Renderer _ingredientRenderer;
    private PhotonView _photonView;

    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    private void Awake()
    {
        InitIngredient();
    }

    private void InitIngredient()
    {
        _modelObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            _modelObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }

        _ingredientMeshFilter = GetComponent<MeshFilter>();
        _ingredientRenderer = GetComponent<Renderer>();
        _photonView = GetComponent<PhotonView>();
    }

    public void InitIngredientData(int TID)
    {
        _data = DataTable.Instance.GetIngredientData(TID);
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            if (modelInfo.TID == _data.TID)
            {
                modelInfo.Model.SetActive(true);
            }
        }

        _photonView.RPC(nameof(RPC_InitIngredientData), RpcTarget.Others, TID);
    }

    [PunRPC]
    public void RPC_InitIngredientData(int TID)
    {
        _data = DataTable.Instance.GetIngredientData(TID);

        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            if (modelInfo.TID == _data.TID)
            {
                modelInfo.Model.SetActive(true);
            }
        }
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
