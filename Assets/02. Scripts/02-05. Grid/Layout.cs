using UnityEngine;
using System.Collections.Generic;

public class Layout : MonoBehaviour
{
    [System.Serializable]
    public class AreaDefinition
    {
        public EAreaType AreaType;
        public List<Vector3Int> GridPositions; // 이 구역에 속하는 셀들의 리스트
    }

    [Tooltip("에디터에서 설정된 모든 구역 정의")]
    [SerializeField]
    private List<AreaDefinition> _allAreaDefinitionList = new List<AreaDefinition>();

    // 런타임에 사용할 구역 맵 (Key: GridPosition, Value: AreaType)
    private Dictionary<Vector3Int, EAreaType> _areaDict;

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
            foreach (var pos in areaDef.GridPositions)
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

    // 특정 그리드 셀의 구역 타입을 반환
    public EAreaType GetAreaTypeAtGridPosition(Vector3Int gridPos)
    {
        if (_areaDict.TryGetValue(gridPos, out EAreaType type))
        {
            return type;
        }
        return EAreaType.None; // 구역이 지정되지 않은 경우
    }

    // 외부에서 요청하는 Dictionary<Vector3Int, bool> availableAreaDict를 제공
    // 이 딕셔너리는 특정 가구 타입(예: 주방 가구)이 배치될 수 있는 모든 유효한 그리드 셀을 나타낼 수 있습니다.
    public Dictionary<Vector3Int, EAreaType> GetAvailableAreaDict()
    {
        return _areaDict;

        //Dictionary<Vector3Int, bool> availableDict = new Dictionary<Vector3Int, bool>();

        //// 그리드 전체를 순회하며 가구 배치 가능 여부 판단
        //GridInfo gridInfo = GetComponent<GridInfo>();
        //if (gridInfo == null)
        //{
        //    Debug.LogError("AreaManager: GridInfo component not found!");
        //    return availableDict;
        //}

        //for (int x = 0; x < gridInfo.GridSize.x; x++)
        //{
        //    for (int z = 0; z < gridInfo.GridSize.y; z++) // Z축이 Y축 역할
        //    {
        //        Vector3Int currentGridPos = new Vector3Int(x, 0, z);
        //        EAreaType currentAreaType = GetAreaTypeAtGridPosition(currentGridPos);

        //        // 현재 셀의 구역 타입이 가구가 원하는 구역 타입과 일치하고, 해당 구역에 배치 가능 규칙이 있다면 true
        //        bool canPlace = currentAreaType == furnitureDesiredArea;

        //        availableDict.Add(currentGridPos, canPlace);
        //    }
        //}
        //return availableDict;
    }
}