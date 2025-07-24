using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[Serializable]
public class PotionAppearance
{
    public int TID;
    public Mesh BottleMesh;
    public Mesh LiquidMesh;
    public Material LiquidMaterial;
}

public class PotionItem : MonoBehaviour, IItem
{
    private PotionData _potionData;
    public PotionData PotionData => _potionData;

    [Foldout("Hierarchy")]
    [SerializeField]
    private MeshFilter _bottleMeshFilter;
    [SerializeField]
    private Renderer _bottleRenderer;
    [SerializeField]
    private MeshFilter _liquidMeshFilter;
    [SerializeField]
    private Renderer _liquidRenderer;


    [Foldout("Project")]
    [SerializeField]
    private List<PotionAppearance> _potionAppearanceList = new List<PotionAppearance>();
    private Dictionary<int, PotionAppearance> _potionAppearanceDict;

    private Light _pointLight;
    private ParticleSystem _particles;
    private MaterialPropertyBlock _mpb;

    [Foldout("Component")]
    private PhotonView _photonView;

    private void Awake()
    {
        InitPotion();
    }

    private void InitPotion()
    {
        _potionAppearanceDict = new Dictionary<int, PotionAppearance>();
        foreach (var potionAppearance in _potionAppearanceList)
        {
            _potionAppearanceDict.Add(potionAppearance.TID, potionAppearance);
        }

        _pointLight = GetComponentInChildren<Light>();
        _mpb = new MaterialPropertyBlock();
        _particles = GetComponentInChildren<ParticleSystem>(true);

        _photonView = GetComponent<PhotonView>();
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
        InitPotionAppearance(TID);
        InitVFX();
    }

    private void InitVFX()
    {
        _bottleRenderer.GetPropertyBlock(_mpb);
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
        _bottleRenderer.SetPropertyBlock(_mpb);
    }

    private void InitPotionAppearance(int TID)
    {
        _bottleMeshFilter.mesh = _potionAppearanceDict[TID].BottleMesh;
        _liquidMeshFilter.mesh = _potionAppearanceDict[TID].LiquidMesh;
        _liquidRenderer.material = _potionAppearanceDict[TID].LiquidMaterial;
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
