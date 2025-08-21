using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : NetworkBehaviour, IItem
{
    [SyncVar(hook = nameof(OnPotionItemTIDUpdated))]
    private int _potionTID;
    public int PotionTID => _potionTID;

    private PotionData _potionData;
    public PotionData PotionData => _potionData;

    private GameObject _currentModel;
    private Renderer _potionBottleRenderer;
    private Light _pointLight;
    private ParticleSystem _particles;

    private MaterialPropertyBlock _mpb;

    [SerializeField]
    private List<ModelOnTID> _potionModelList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _potionModelDict;

    [SerializeField]
    private GameObject _models;
    private Coroutine _visibleRoutine;

    private void Awake()
    {
        InitPotion();
    }

    private void InitPotion()
    {
        _potionModelDict = new Dictionary<int, GameObject>();
        foreach (var potionModel in _potionModelList)
        {
            potionModel.Model.SetActive(false);
            _potionModelDict.Add(potionModel.TID, potionModel.Model);
        }

        _mpb = new MaterialPropertyBlock();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!ReferenceEquals(_visibleRoutine, null))
        {
            StopCoroutine(_visibleRoutine);
        }
        if (gameObject.activeInHierarchy)
        {
            _visibleRoutine = StartCoroutine(VisibleRoutine());
        }
    }

    private void OnPotionItemTIDUpdated(int oldValue, int newValue)
    {
        ClientUpdatePotionData();
    }

    

    [Server]
    public void ServerUpdatePotionData(int TID)
    {
        _potionTID = TID;
        _potionData = DataTable.Instance.GetPotionData(TID);
    }

    private void ClientUpdatePotionData()
    {
        // 클라이언트에서 초기화 시 SyncVar로 받은 TID를 사용
        _potionData = DataTable.Instance.GetPotionData(_potionTID);

        ResetModel();

        if (_potionModelDict.TryGetValue(_potionData.TID, out _currentModel))
        {
            _currentModel.SetActive(true);
            UpdateComponent();
            UpdateVFX();
        }
        else
        {
            Debug.LogError($"포션 TID {_potionData.TID}에 대응하는 모델이 딕셔너리에 존재하지 않습니다.");
        }
    }
    private void ResetModel()
    {
        if (_potionModelDict == null) return;
        foreach (var modelInfo in _potionModelDict)
        {
            modelInfo.Value.SetActive(false);
        }
    }

    private void UpdateComponent()
    {
        _potionBottleRenderer = _currentModel.GetComponent<Renderer>();
        _pointLight = _currentModel.GetComponentInChildren<Light>();
        _particles = _currentModel.GetComponentInChildren<ParticleSystem>(true);
    }

    private void UpdateVFX()
    {
        if (ReferenceEquals(_pointLight, null))
        {
            Debug.LogError($"포션 효과를 위한 PointLight가 null입니다.");
            return;
        }

        if (ReferenceEquals(_particles, null))
        {
            Debug.LogError($"포션 효과를 위한 PointLight가 null입니다.");
            return;
        }

        _potionBottleRenderer.GetPropertyBlock(_mpb);
        switch (_potionData.Tier)
        {
            case 1:
                _mpb.SetFloat("_Epic", 0);
                _mpb.SetFloat("_Efect_emission", 0);
                _pointLight.enabled = false;
                _particles.gameObject.SetActive(false);
                break;
            case 2:
                _mpb.SetFloat("_Epic", 1);
                _mpb.SetFloat("_Efect_emission", 1.5f);
                _pointLight.enabled = true;
                _particles.gameObject.SetActive(false);
                break;
            case 3:
                _mpb.SetFloat("_Epic", 1);
                _mpb.SetFloat("_Efect_emission", 1.5f);
                _pointLight.enabled = true;
                _particles.gameObject.SetActive(true);
                break;
            default:
                _mpb.SetFloat("_Epic", 0);
                _mpb.SetFloat("_Efect_emission", 0);
                _pointLight.enabled = false;
                _particles.gameObject.SetActive(false);
                break;
        }
        _potionBottleRenderer.SetPropertyBlock(_mpb);
    }

    public EInputType GetInputType()
    {
        return EInputType.Potion;
    }

    public int GetTID()
    {
        return _potionTID;
    }


    private IEnumerator VisibleRoutine()
    {
        _models.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        _models.SetActive(true);
    }
}