using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.PackageManager;

[CustomEditor(typeof(Layout))]
public class AreaEditor : Editor
{
    private Layout _gridLayout;
    private GridInfo _gridInfo;

    private EAreaType _selectedAreaType = EAreaType.None; // 현재 선택된 구역 타입
    private bool _isDrawing = false; // 드래그 중인지 여부
    private Vector3Int _startGridPos; // 드래그 시작 그리드 좌표

    private int _toolbarIndex = 0;
    private string[] _toolbarNames;

    private void OnEnable()
    {
        _gridLayout = (Layout)target;
        _gridInfo = _gridLayout.GetComponent<GridInfo>();

        if (_gridInfo == null)
        {
            Debug.LogError("AreaEditor requires a GridInfo component on the same GameObject!");
        }

        // EAreaType enum의 이름들을 가져와 툴바 이름으로 사용
        _toolbarNames = System.Enum.GetNames(typeof(EAreaType));
        // None은 그리지 않으므로 첫 번째 (None)를 제외하고 시작
        _toolbarIndex = 1; // Hall부터 시작
        _selectedAreaType = (EAreaType)_toolbarIndex;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // AreaManager의 기본 인스펙터 그리기

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("--- Area Editor Tools ---", EditorStyles.boldLabel);

        // 구역 타입 선택 툴바
        _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _toolbarNames);
        _selectedAreaType = (EAreaType)_toolbarIndex;

        EditorGUILayout.HelpBox("Select an area type above, then hold SHIFT and drag on the grid to draw areas. Hold CTRL to erase areas.", MessageType.Info);

        // 모든 구역 정의 리스트 초기화 버튼 (주의해서 사용)
        if (GUILayout.Button("Clear All Areas (CAUTION!)"))
        {
            if (EditorUtility.DisplayDialog("Clear All Areas?",
                                            "Are you sure you want to clear all defined areas? This cannot be undone!",
                                            "Clear All", "Cancel"))
            {
                _gridLayout.GetType().GetField("_allAreaDefinitionList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                           .SetValue(_gridLayout, new List<AreaDefinition>());
                EditorUtility.SetDirty(_gridLayout);
                Debug.Log("All areas cleared.");
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_gridInfo == null || _gridLayout == null) return;

        // 마우스 입력 처리
        Event guiEvent = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        Plane plane = new Plane(Vector3.up, _gridInfo.transform.position.y); // 그리드가 있는 평면

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldMousePos = ray.GetPoint(distance);
            Vector3Int currentGridPos = _gridInfo.WorldToGrid(worldMousePos);

            // 그리드 범위를 벗어나면 처리하지 않음
            if (!_gridInfo.IsValidGridPosition(currentGridPos))
            {
                if (_isDrawing)
                {
                    _isDrawing = false; // 드래그 중 범위를 벗어나면 드래그 종료
                    SceneView.RepaintAll();
                }
                return;
            }

            // 드로잉 중인 셀 강조 표시
            DrawCellHighlight(currentGridPos, ObjectColors.AreaHandleColor);

            // SHIFT 또는 CTRL 키를 누른 상태에서 드래그 처리
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && (guiEvent.shift || guiEvent.control))
            {
                _isDrawing = true;
                _startGridPos = currentGridPos;
                guiEvent.Use(); // 이벤트 소비
            }
            else if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && _isDrawing)
            {
                _isDrawing = false;
                RecordAreaChange(_startGridPos, currentGridPos, guiEvent.control); // CTRL 누르면 지우기
                guiEvent.Use();
            }
            else if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && _isDrawing)
            {
                // 드래그 중인 임시 영역 그리기
                DrawTempArea(_startGridPos, currentGridPos, ObjectColors.AreaActiveDrawColor);
                SceneView.RepaintAll(); // 씬 뷰 강제 업데이트
            }
        }

        // 현재 정의된 모든 구역 그리기 (항상 보이도록)
        DrawDefinedAreas();

        // 씬 뷰를 다시 그리도록 요청 (마우스 움직임에 따라 하이라이트/드로잉 업데이트)
        SceneView.RepaintAll();
    }

    // 단일 셀 하이라이트
    private void DrawCellHighlight(Vector3Int gridPos, Color color)
    {
        Vector3 worldCenter = _gridInfo.GridToWorld(gridPos);
        Handles.color = color;
        Handles.CubeHandleCap(0, worldCenter, Quaternion.identity, _gridInfo.CellSize * 0.9f, EventType.Repaint);
    }

    // 임시 영역 그리기
    private void DrawTempArea(Vector3Int start, Vector3Int end, Color color)
    {
        Vector3Int min = new Vector3Int(Mathf.Min(start.x, end.x), 0, Mathf.Min(start.z, end.z));
        Vector3Int max = new Vector3Int(Mathf.Max(start.x, end.x), 0, Mathf.Max(start.z, end.z));

        Handles.color = color;
        for (int x = min.x; x <= max.x; x++)
        {
            for (int z = min.z; z <= max.z; z++)
            {
                Vector3Int current = new Vector3Int(x, 0, z);
                Vector3 worldCenter = _gridInfo.GridToWorld(current);
                Handles.CubeHandleCap(0, worldCenter, Quaternion.identity, _gridInfo.CellSize * 0.9f, EventType.Repaint);
            }
        }
    }

    // 정의된 모든 구역을 씬 뷰에 그리기
    private void DrawDefinedAreas()
    {
        // 리플렉션을 사용하여 private 필드에 접근
        List<AreaDefinition> allAreaDefinitions = (List<AreaDefinition>)_gridLayout.GetType().GetField("_allAreaDefinitionList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_gridLayout);

        if (allAreaDefinitions == null) return;

        foreach (var areaDef in allAreaDefinitions)
        {
            // 구역 타입에 따라 색상 변경 (예시)
            Color areaColor = GetColorForAreaType(areaDef.AreaType);
            Handles.color = areaColor;

            foreach (var pos in areaDef.GridPositionList)
            {
                Vector3 worldCenter = _gridInfo.GridToWorld(pos);
                // BoxHandles.DrawWireCube 대신 Handles.CubeHandleCap 사용 (Solid)
                Handles.CubeHandleCap(0, worldCenter, Quaternion.identity, _gridInfo.CellSize * 0.9f, EventType.Repaint);
            }
        }
    }

    // 구역 타입별 색상 매핑 (에디터 전용)
    private Color GetColorForAreaType(EAreaType type)
    {
        switch (type)
        {
            case EAreaType.Hall: return ObjectColors.AreaGreenColor;    // 초록
            case EAreaType.Kitchen: return ObjectColors.AreaOrangeColor; // 주황
            case EAreaType.Storage: return ObjectColors.AreaGrayColor; // 회색
            default: return ObjectColors.AreaDefaultColor; // 기본
        }
    }


    // 영역 변경을 기록 (새로 추가 또는 삭제)
    private void RecordAreaChange(Vector3Int start, Vector3Int end, bool eraseMode)
    {
        // 명령 취소/다시 실행을 위해 변경 사항 기록
        Undo.RecordObject(_gridLayout, eraseMode ? "Erase Area" : "Draw Area");

        // 리플렉션을 사용하여 private 필드에 접근
        List<AreaDefinition> allAreaDefinitions = (List<AreaDefinition>)_gridLayout.GetType().GetField("_allAreaDefinitionList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_gridLayout);

        // 현재 선택된 구역 타입에 해당하는 AreaDefinition을 찾거나 새로 생성
        AreaDefinition currentAreaDef = allAreaDefinitions.Find(def => def.AreaType == _selectedAreaType);
        if (currentAreaDef == null && !eraseMode) // 지우기 모드가 아니면서 해당 타입의 정의가 없으면 새로 생성
        {
            currentAreaDef = new AreaDefinition { AreaType = _selectedAreaType, GridPositionList = new List<Vector3Int>() };
            allAreaDefinitions.Add(currentAreaDef);
        }

        Vector3Int min = new Vector3Int(Mathf.Min(start.x, end.x), 0, Mathf.Min(start.z, end.z));
        Vector3Int max = new Vector3Int(Mathf.Max(start.x, end.x), 0, Mathf.Max(start.z, end.z));

        for (int x = min.x; x <= max.x; x++)
        {
            for (int z = min.z; z <= max.z; z++)
            {
                Vector3Int pos = new Vector3Int(x, 0, z);

                if (!_gridInfo.IsValidGridPosition(pos)) continue; // 유효하지 않은 그리드 좌표는 스킵

                if (eraseMode)
                {
                    // CTRL (지우기) 모드: 모든 구역 정의에서 해당 위치를 제거
                    foreach (var def in allAreaDefinitions)
                    {
                        def.GridPositionList.Remove(pos);
                    }
                }
                else
                {
                    // SHIFT (그리기) 모드: 다른 구역에서 해당 위치를 제거하고 현재 구역에 추가
                    foreach (var def in allAreaDefinitions)
                    {
                        if (def.AreaType != _selectedAreaType)
                        {
                            def.GridPositionList.Remove(pos);
                        }
                    }
                    if (currentAreaDef != null && !currentAreaDef.GridPositionList.Contains(pos))
                    {
                        currentAreaDef.GridPositionList.Add(pos);
                    }
                }
            }
        }

        // 빈 구역 정의 제거
        allAreaDefinitions.RemoveAll(def => def.GridPositionList.Count == 0);

        // 변경 사항을 저장하도록 유니티에 알림
        EditorUtility.SetDirty(_gridLayout);
    }
}