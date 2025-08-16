using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientItem : NetworkBehaviour, IItem
{
    public event Action<int> OnItemTIDUpdated;
    public event Action<bool> OnItemHighlighted;

    [SyncVar(hook = nameof(OnIngredientItemTIDUpdated))]
    private int _ingredientTID;
    public int IngredientTID => _ingredientTID;

    private IngredientData _ingredientData;
    public IngredientData IngredientData => _ingredientData;

    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    [SerializeField]
    private GameObject _models;
    private Coroutine _visibleRoutine;

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
        _models.SetActive(false);
        if (!ReferenceEquals(_visibleRoutine, null))
        {
            StopCoroutine(_visibleRoutine);
        }
        _visibleRoutine = StartCoroutine(VisibleRoutine());
    }

    private void OnIngredientItemTIDUpdated(int oldValue, int newValue)
    {
        ClientUpdateIngredientData();
    }


    [Server]
    public void ServerUpdateIngredientData(int TID)
    {
        _ingredientTID = TID;
        _ingredientData = DataTable.Instance.GetIngredientData(TID);
    }

    private void ClientUpdateIngredientData()
    {
        // 클라이언트에서 초기화 시 SyncVar로 받은 TID를 사용해 데이터 로드 및 모델 활성화
        _ingredientData = DataTable.Instance.GetIngredientData(_ingredientTID);
        ActivateModelForTID(_ingredientData.TID);
        OnItemTIDUpdated?.Invoke(_ingredientData.AvailableMachineTID);
    }

    private void ActivateModelForTID(int tid)
    {
        if (_modelObjectDic == null) return;
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
        }

        if (_modelObjectDic.TryGetValue(tid, out GameObject modelToActivate))
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

    private IEnumerator VisibleRoutine()
    {
        yield return new WaitForSeconds(.05f);
        _models.gameObject.SetActive(true);
    }

    public void HandleIngredientHighlighted(bool isActive)
    {
        OnItemHighlighted?.Invoke(isActive);
    }
}