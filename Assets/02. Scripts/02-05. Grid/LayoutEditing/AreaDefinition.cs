using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaDefinition
{
    public EAreaType AreaType;
    public List<Vector3Int> GridPositionList; // 이 구역에 속하는 셀들의 리스트
}
