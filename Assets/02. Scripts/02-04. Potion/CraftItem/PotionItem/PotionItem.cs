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

public class PotionItem : MonoBehaviour, IItem
{
    private PotionData _potionData;
    public PotionData PotionData => _potionData;

    private MeshFilter _meshFilter;
    private PhotonView _photonView;

    [Foldout("Project")]
    [SerializeField]
    private List<MeshOnTID> _meshList = new List<MeshOnTID>();
    private Dictionary<int, Mesh> _meshDict;

    private Renderer _material;
    private Light _pointLight;
    private ParticleSystem _particles;
    private MaterialPropertyBlock _mpb;

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

        _pointLight = GetComponentInChildren<Light>();
        _mpb = new MaterialPropertyBlock();
        _material = transform.GetChild(0).GetComponent<Renderer>();
        _particles = GetComponentInChildren<ParticleSystem>(true);
    }

    public void InitPotionData(int TID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Potion 객체 생성 후 내부 데이터 초기화는 Master만 가능합니다.");
        }
        RPC_InitPotionData(TID);
        _photonView.RPC(nameof(RPC_InitPotionData), RpcTarget.Others, TID);
    }

    [PunRPC]
    public void RPC_InitPotionData(int TID)
    {
        _potionData = DataTable.Instance.GetPotionData(TID);
        _meshFilter.mesh = _meshDict[TID];
        InitVFX();
    }

    private void InitVFX()
    {
        _material.GetPropertyBlock(_mpb);
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
        _material.SetPropertyBlock(_mpb);
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
