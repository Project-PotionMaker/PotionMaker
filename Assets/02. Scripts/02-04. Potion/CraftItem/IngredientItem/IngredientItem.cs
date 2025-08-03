using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IngredientItem : NetworkBehaviour, IItem
{
    [SyncVar(hook = nameof(OnIngredientItemTIDUpdated))]
    private int _ingredientTID;
    public int IngredientTID => _ingredientTID;

    private IngredientData _data;
    public IngredientData Data => _data;

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
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void OnIngredientItemTIDUpdated(int oldValue, int newValue)
    {
        ClientUpdateIngredientData();
    }


    [Server]
    public void ServerUpdateIngredientData(int TID)
    {
        _ingredientTID = TID;
    }

    private void ClientUpdateIngredientData()
    {
        // 클라이언트에서 초기화 시 SyncVar로 받은 TID를 사용해 데이터 로드 및 모델 활성화
        _data = DataTable.Instance.GetIngredientData(_ingredientTID);

        // TID에 맞는 모델을 한 번만 활성화
        if (_modelObjectDic.TryGetValue(_data.TID, out GameObject modelToActivate))
        {
            modelToActivate.SetActive(true);
        }
    }

    public EInputType GetInputType()
    {
        return EInputType.Ingredient;
    }

    public int GetTID()
    {
        return _ingredientTID;
    }
}