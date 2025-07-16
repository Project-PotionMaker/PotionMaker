using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Output : MonoBehaviour
{
    private EInputType _currentInputType;
    public EInputType CurrentInputType => _currentInputType;

    private OutputData _outputData;
    public OutputData OutputData => _outputData;

    private PotionData _potionData = null;
    public PotionData PotionData => _potionData;

    private string _outputCode;
    public string OutputCode => _outputCode;

    private MeshFilter _meshFilter;


    [Foldout("Project")]
    [SerializeField]
    private List<(EInputType, Mesh)> _meshList;

    private Dictionary<EInputType, Mesh> _meshDict;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _meshDict = new Dictionary<EInputType, Mesh>();
        foreach (var meshInfo in _meshList)
        {
            _meshDict.Add(meshInfo.Item1, meshInfo.Item2);
        }

        _meshFilter = GetComponent<MeshFilter>();
    }

    // 병입기를 제외한 모든 조리기구에서 조리가 완료되었을 때 호출해야하는 메서드
    public void ChangeState(EInputType newInputType, OutputData newData, string newOutputCode)
    {
        _currentInputType = newInputType;
        _outputData = newData;
        _outputCode = newOutputCode;
        _meshFilter.mesh = _meshDict[newInputType];
    }

    // 병입기에서 조리가 완료되었을 때 호출해야하는 메서드
    public void CreatePotion(PotionData potionData, Mesh potionMesh)
    {
        _currentInputType = EInputType.BottlerOutput;
        _outputData = null;
        _potionData = potionData;
        _meshFilter.mesh = potionMesh;
    }
}
