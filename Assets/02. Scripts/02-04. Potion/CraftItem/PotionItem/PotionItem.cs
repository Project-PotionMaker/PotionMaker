using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class PotionItem : MonoBehaviour, IItem
{
    private PotionData _potionData;
    public PotionData PotionData => _potionData;

    [Header("Component")]
    private GameObject _currentModel;
    private PhotonView _photonView;
    private Renderer _potionBottleRenderer;
    private Light _pointLight;
    private ParticleSystem _particles;

    private MaterialPropertyBlock _mpb;

    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _potionModelList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _potionModelDict;

    private void Awake()
    {
        InitPotionItem();
    }

    private void InitPotionItem()
    {
        _photonView = GetComponent<PhotonView>();
        _mpb = new MaterialPropertyBlock();
        InitPotionModelDictionary();
    }

    private void InitPotionModelDictionary()
    {
        _potionModelDict = new Dictionary<int, GameObject>();
        foreach (var potionModel in _potionModelList)
        {
            potionModel.Model.SetActive(false);
            _potionModelDict.Add(potionModel.TID, potionModel.Model);
        }
    }

    public void UpdatePotionData(int TID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Potion 객체 생성 후 내부 데이터 초기화는 Master만 가능합니다.");
        }
        RPC_UpdatePotionData(TID);
        _photonView.RPC(nameof(RPC_UpdatePotionData), RpcTarget.Others, TID);
    }

    [PunRPC]
    public void RPC_UpdatePotionData(int TID)
    {
        _potionData = DataTable.Instance.GetPotionData(TID);
        if (!ReferenceEquals(_currentModel, null))
        {
            _currentModel.SetActive(false);
        }

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

    private void UpdateComponent()
    {
        _potionBottleRenderer = _currentModel.GetComponent<Renderer>();
        _pointLight = _currentModel.GetComponentInChildren<Light>();
        _particles = _currentModel.GetComponentInChildren<ParticleSystem>();
    }

    private void UpdateVFX()
    {
        _potionBottleRenderer.GetPropertyBlock(_mpb);
        switch (_potionData.Tier)
        {
            case 1:
                _mpb.SetFloat("_Epic", 0);
                _mpb.SetFloat("_Efect_emission", 0);
                _pointLight.enabled = false;
                break;
            case 2:
                _mpb.SetFloat("_Epic", 1);
                _mpb.SetFloat("_Efect_emission", 1.5f);
                break;
            case 3:
                _particles.gameObject.SetActive(true);
                break;
            default:
                _mpb.SetFloat("_Epic", 0);
                _mpb.SetFloat("_Efect_emission", 0);
                _pointLight.enabled = false;
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
        return _potionData.TID;
    }
}
