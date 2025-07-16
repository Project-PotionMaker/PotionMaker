using UnityEngine;

public class Potion : MonoBehaviour
{
    private PotionData _potionData;
    public PotionData PotionData => _potionData;

    private MeshFilter _meshFilter;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void InitPotionData(PotionData potionData, Mesh potionMesh)
    {
        _potionData = potionData;
        _meshFilter.mesh = potionMesh;
    }
}
