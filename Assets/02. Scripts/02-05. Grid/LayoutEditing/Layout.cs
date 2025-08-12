using UnityEngine;
using System.Collections.Generic;

public class Layout : MonoBehaviour
{
    [Tooltip("에디터에서 설정된 모든 구역 정의")]
    [SerializeField]
    private List<AreaDefinition> _allAreaDefinitionList = new List<AreaDefinition>();
    public ReadOnlyList<AreaDefinition> AllAreaDefinitionList => new ReadOnlyList<AreaDefinition>(_allAreaDefinitionList);
    // 런타임에 사용할 구역 맵 (Key: GridPosition, Value: AreaType)
    private Dictionary<Vector3Int, EAreaType> _areaDict;

    [SerializeField]
    private Vector3Int _enterDoorPosition;
    public Vector3Int EnterDoorPosition => _enterDoorPosition;

    [SerializeField]
    private Vector3Int _exitDoorPosition;
    public Vector3Int ExitDoorPosition => _exitDoorPosition;

    [SerializeField]
    private Vector3Int _cashierSpawnPosition;
    public Vector3Int CashierSpawnPosition => _cashierSpawnPosition;

    void Awake()
    {
        InitializeAreaMap();
    }

    // allAreaDefinitions 리스트를 기반으로 areaMap을 초기화합니다.
    private void InitializeAreaMap()
    {
        _areaDict = new Dictionary<Vector3Int, EAreaType>();
        foreach (var areaDef in _allAreaDefinitionList)
        {
            foreach (var pos in areaDef.GridPositionList)
            {
                if (_areaDict.ContainsKey(pos))
                {
                    Debug.LogWarning($"AreaManager: Grid position {pos} is assigned to multiple areas. Overwriting with {areaDef.AreaType}.");
                    _areaDict[pos] = areaDef.AreaType; // 중복 할당 시 마지막 구역으로 덮어쓰기
                }
                else
                {
                    _areaDict.Add(pos, areaDef.AreaType);
                }
            }
        }
        Debug.Log($"AreaManager: Initialized area map with {_areaDict.Count} grid positions.");
    }

    public Dictionary<Vector3Int, EAreaType> GetAvailableAreaDict()
    {
        return _areaDict;
    }
}