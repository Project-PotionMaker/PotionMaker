using UnityEngine;

public class GridInfo : MonoBehaviour
{
    [Tooltip("그리드의 기본 셀 크기 (일반적으로 1x1)")]
    public float CellSize = 1f;

    [Tooltip("그리드 전체의 가로/세로 셀 개수. Plane의 Material DefaultSize.x/y 값으로 자동 설정됩니다.")]
    public Vector2Int GridSize; // 예: (10, 10)

    [Tooltip("그리드의 기준점 (일반적으로 Plane의 좌하단 코너)")]
    public Vector3 Origin;

    // 월드 좌표를 그리드 셀 좌표 (Vector3Int)로 변환
    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - Origin;
        int x = Mathf.FloorToInt(localPos.x / CellSize);
        int y = Mathf.FloorToInt(localPos.z / CellSize); // Z축을 y축으로 사용 (Top-down view)
        return new Vector3Int(x, 0, y); // Y는 0으로 고정 (2D 그리드)
    }

    // 그리드 셀 좌표를 월드 중심 좌표 (Vector3)로 변환
    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        float worldX = Origin.x + (gridPos.x * CellSize) + (CellSize / 2f);
        float worldZ = Origin.z + (gridPos.z * CellSize) + (CellSize / 2f);

        Debug.Log(gridPos + " " + new Vector3(worldX, Origin.y, worldZ));
        return new Vector3(worldX, Origin.y, worldZ); // Y는 Plane의 Y좌표
    }

    // 그리드 좌표가 유효한지 확인
    public bool IsValidGridPosition(Vector3Int gridPos)
    {
        return gridPos.x >= -GridSize.x / 2 && gridPos.x < GridSize.x / 2 &&
               gridPos.z >= -GridSize.y / 2 && gridPos.z < GridSize.y / 2;
    }
}