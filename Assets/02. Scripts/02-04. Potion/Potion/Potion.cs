using Photon.Pun;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private PotionData _potionData;
    public PotionData PotionData => _potionData;

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

    public void InitPotionData(PotionData potionData, Mesh potionMesh)
    {
        _photonView.RPC(nameof(RPC_InitPotionData), RpcTarget.All, potionData, potionMesh);
    }

    [PunRPC]
    public void RPC_InitPotionData(PotionData potionData, Mesh potionMesh)
    {
        _potionData = potionData;
        _meshFilter.mesh = potionMesh;
    }
}
