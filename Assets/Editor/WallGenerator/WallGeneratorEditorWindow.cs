using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class WallGeneratorEditorWindow : EditorWindow
{
    private int startX = 0;
    private int startY = 0;
    private int width = 5;
    private int height = 5;

    private GameObject wallPrefab;
    private GameObject wallsParent;

    private const float CELL_SIZE = 1f;

    private GameObject selectedPlane;
    private bool useSelectedPlane = false;

    [MenuItem("Tools/1x1 Wall Generator (Rotation Fixed)")] // 메뉴 이름 변경
    public static void ShowWindow()
    {
        GetWindow<WallGeneratorEditorWindow>("1x1 Wall Generator (Rotation Fixed)");
    }

    private void OnGUI()
    {
        GUILayout.Label("1x1 Wall Generator Settings", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        wallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", wallPrefab, typeof(GameObject), false);
        if (wallPrefab == null)
        {
            EditorGUILayout.HelpBox("벽으로 사용할 프리팹을 할당해주세요. (피벗이 중앙에 있는 오브젝트 권장)", MessageType.Warning);
        }

        EditorGUILayout.Space();
        wallsParent = (GameObject)EditorGUILayout.ObjectField("Walls Parent Object (Optional)", wallsParent, typeof(GameObject), true);
        if (wallsParent == null)
        {
            EditorGUILayout.HelpBox("생성된 벽들을 묶을 부모 오브젝트를 지정할 수 있습니다. 비워두면 씬의 루트에 'GeneratedWalls'로 생성됩니다.", MessageType.Info);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Grid Area Definition", EditorStyles.boldLabel);
        useSelectedPlane = EditorGUILayout.Toggle("Use Selected Plane as Area", useSelectedPlane);

        if (useSelectedPlane)
        {
            GameObject currentSelection = Selection.activeGameObject;
            if (currentSelection != null && currentSelection.GetComponent<MeshFilter>() != null &&
                currentSelection.GetComponent<MeshFilter>().sharedMesh != null &&
                currentSelection.GetComponent<MeshFilter>().sharedMesh.name == "Plane")
            {
                selectedPlane = currentSelection;
                EditorGUILayout.ObjectField("Detected Plane", selectedPlane, typeof(GameObject), true);
                EditorGUILayout.HelpBox("선택된 Plane의 경계를 기준으로 벽이 생성됩니다.", MessageType.Info);
            }
            else
            {
                selectedPlane = null;
                EditorGUILayout.HelpBox("Plane 오브젝트를 선택해주세요. (또는 'Use Selected Plane' 해제)", MessageType.Warning);
            }
        }
        else
        {
            GUILayout.Label("Manual Grid Area (Inside)", EditorStyles.boldLabel);
            startX = EditorGUILayout.IntField("Start X (Grid Coord)", startX);
            startY = EditorGUILayout.IntField("Start Y (Grid Coord)", startY);
            width = EditorGUILayout.IntField("Width (Cells)", width);
            height = EditorGUILayout.IntField("Height (Cells)", height);

            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Outer Walls"))
        {
            if (wallPrefab == null)
            {
                EditorUtility.DisplayDialog("경고", "벽 프리팹을 할당해주세요!", "확인");
                return;
            }

            if (useSelectedPlane)
            {
                if (selectedPlane != null)
                {
                    GenerateWallsAroundPlane(selectedPlane);
                }
                else
                {
                    EditorUtility.DisplayDialog("경고", "벽을 생성하려면 Plane 오브젝트를 선택해야 합니다.", "확인");
                }
            }
            else
            {
                GenerateWallsManually();
            }
        }

        if (GUILayout.Button("Clear All Generated Walls"))
        {
            ClearAllWalls();
        }
    }

    private void GenerateWallsAroundPlane(GameObject plane)
    {
        ClearAllWalls();

        if (wallsParent == null)
        {
            wallsParent = new GameObject("GeneratedWalls");
            Undo.RegisterCreatedObjectUndo(wallsParent, "Create Walls Parent");
        }

        Renderer planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Selected Plane does not have a Renderer component.");
            return;
        }

        Bounds bounds = planeRenderer.bounds;

        int actualStartX = Mathf.RoundToInt(bounds.min.x / CELL_SIZE);
        int actualStartY = Mathf.RoundToInt(bounds.min.z / CELL_SIZE);

        int actualEndX = Mathf.RoundToInt(bounds.max.x / CELL_SIZE) - 1;
        int actualEndY = Mathf.RoundToInt(bounds.max.z / CELL_SIZE) - 1;

        int actualWidth = (actualEndX - actualStartX) + 1;
        int actualHeight = (actualEndY - actualStartY) + 1;

        GenerateWallsCommonLogic(actualStartX, actualStartY, actualWidth, actualHeight);
    }

    private void GenerateWallsManually()
    {
        ClearAllWalls();

        if (wallsParent == null)
        {
            wallsParent = new GameObject("GeneratedWalls");
            Undo.RegisterCreatedObjectUndo(wallsParent, "Create Walls Parent");
        }
        GenerateWallsCommonLogic(startX, startY, width, height);
    }

    private void GenerateWallsCommonLogic(int currentStartX, int currentStartY, int currentWidth, int currentHeight)
    {
        int undoGroupIndex = Undo.GetCurrentGroup();

        int minX = currentStartX;
        int minY = currentStartY;
        int maxX = currentStartX + currentWidth - 1;
        int maxY = currentStartY + currentHeight - 1;

        // --- 상단 벽 (+Z 방향) ---
        // 벽 프리팹의 Z축이 길이가 아니라면 X축으로 90도 회전하여 Z축을 따라 길게 놓아야 합니다.
        for (int x = minX; x <= maxX; x++)
        {
            Vector3 spawnPos = GetWallSpawnPosition(x, maxY + 1, WallOrientation.AlongXAxis);
            InstantiateAndParentWall(spawnPos, Quaternion.Euler(0, 90, 0)); // 변경: identity -> Y 90도 회전
        }

        // --- 하단 벽 (-Z 방향) ---
        // 벽 프리팹의 Z축이 길이가 아니라면 X축으로 90도 회전하여 Z축을 따라 길게 놓아야 합니다.
        for (int x = minX; x <= maxX; x++)
        {
            Vector3 spawnPos = GetWallSpawnPosition(x, minY, WallOrientation.AlongXAxis);
            InstantiateAndParentWall(spawnPos, Quaternion.Euler(0, 90, 0)); // 변경: identity -> Y 90도 회전
        }

        // --- 좌측 벽 (-X 방향) ---
        // 벽 프리팹의 X축이 길이가 아니라면 Z축으로 90도 회전하여 X축을 따라 길게 놓아야 합니다.
        for (int y = minY; y <= maxY; y++)
        {
            Vector3 spawnPos = GetWallSpawnPosition(minX, y, WallOrientation.AlongZAxis);
            InstantiateAndParentWall(spawnPos, Quaternion.identity); // 변경: Y 90도 회전 -> identity
        }

        // --- 우측 벽 (+X 방향) ---
        // 벽 프리팹의 X축이 길이가 아니라면 Z축으로 90도 회전하여 X축을 따라 길게 놓아야 합니다.
        for (int y = minY; y <= maxY; y++)
        {
            Vector3 spawnPos = GetWallSpawnPosition(maxX + 1, y, WallOrientation.AlongZAxis);
            InstantiateAndParentWall(spawnPos, Quaternion.identity); // 변경: Y 90도 회전 -> identity
        }

        Undo.SetCurrentGroupName("Generate Outer Walls");
        Undo.CollapseUndoOperations(undoGroupIndex);

        Debug.Log($"Generated outer walls for area ({currentStartX},{currentStartY}) to ({maxX},{maxY}) with size ({currentWidth}x{currentHeight})");
    }

    private enum WallOrientation
    {
        AlongXAxis, // 벽이 X축 방향으로 길게 놓임 (이전: 프리팹의 Z축이 길이가 됨, 즉 회전 없음 -> 변경: Y 90도 회전 필요)
        AlongZAxis  // 벽이 Z축 방향으로 길게 놓임 (이전: 프리팹의 X축이 길이가 됨, 즉 Y 90도 회전 필요 -> 변경: 회전 없음)
    }

    private GameObject InstantiateAndParentWall(Vector3 position, Quaternion rotation)
    {
        GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
        wall.transform.position = position;
        wall.transform.rotation = rotation;

        Undo.SetTransformParent(wall.transform, wallsParent.transform, "Parent Wall");
        return wall;
    }

    private Vector3 GetWallSpawnPosition(int xGrid, int yGrid, WallOrientation orientation)
    {
        float xWorld = xGrid * CELL_SIZE;
        float zWorld = yGrid * CELL_SIZE;
        float yWorld = 0;

        if (orientation == WallOrientation.AlongXAxis)
        {
            xWorld += CELL_SIZE / 2f;
        }
        else
        {
            zWorld += CELL_SIZE / 2f;
        }
        return new Vector3(xWorld, yWorld, zWorld);
    }

    private void ClearAllWalls()
    {
        if (wallsParent != null)
        {
            Undo.DestroyObjectImmediate(wallsParent);
            wallsParent = null;
        }
        GameObject existingParent = GameObject.Find("GeneratedWalls");
        if (existingParent != null)
        {
            Undo.DestroyObjectImmediate(existingParent);
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        Selection.selectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        Repaint();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        int drawStartX = startX;
        int drawStartY = startY;
        int drawWidth = width;
        int drawHeight = height;

        int drawEndX;
        int drawEndY;

        if (useSelectedPlane && selectedPlane != null)
        {
            Renderer planeRenderer = selectedPlane.GetComponent<Renderer>();
            if (planeRenderer != null)
            {
                Bounds bounds = planeRenderer.bounds;

                drawStartX = Mathf.RoundToInt(bounds.min.x / CELL_SIZE);
                drawStartY = Mathf.RoundToInt(bounds.min.z / CELL_SIZE);

                drawEndX = Mathf.RoundToInt(bounds.max.x / CELL_SIZE) - 1;
                drawEndY = Mathf.RoundToInt(bounds.max.z / CELL_SIZE) - 1;

                drawWidth = (drawEndX - drawStartX) + 1;
                drawHeight = (drawEndY - drawStartY) + 1;
            }
        }
        drawEndX = drawStartX + drawWidth - 1;
        drawEndY = drawStartY + drawHeight - 1;

        // --- Inside Area (Cyan) ---
        Handles.color = Color.cyan;
        Vector3 insideCenter = new Vector3(
            (drawStartX + drawWidth / 2f) * CELL_SIZE,
            0,
            (drawStartY + drawHeight / 2f) * CELL_SIZE
        );
        Vector3 insideSize = new Vector3(drawWidth * CELL_SIZE, 0.1f, drawHeight * CELL_SIZE);
        Handles.DrawWireCube(insideCenter, insideSize);
        Handles.Label(insideCenter + Vector3.up * 0.5f, "Inside Area");

        // --- Outer Wall Pivot Area (Red) ---
        Handles.color = Color.red;

        float redGizmoWorldMinX = GetWallSpawnPosition(drawStartX, 0, WallOrientation.AlongZAxis).x - CELL_SIZE / 2f;
        float redGizmoWorldMinZ = GetWallSpawnPosition(0, drawStartY, WallOrientation.AlongXAxis).z - CELL_SIZE / 2f;

        float redGizmoWorldMaxX = GetWallSpawnPosition(drawEndX + 1, 0, WallOrientation.AlongZAxis).x + CELL_SIZE / 2f;
        float redGizmoWorldMaxZ = GetWallSpawnPosition(0, drawEndY + 1, WallOrientation.AlongXAxis).z + CELL_SIZE / 2f;

        Vector3 redGizmoCenter = new Vector3(
            (redGizmoWorldMinX + redGizmoWorldMaxX) / 2f,
            0,
            (redGizmoWorldMinZ + redGizmoWorldMaxZ) / 2f
        );

        Vector3 redGizmoSize = new Vector3(
            redGizmoWorldMaxX - redGizmoWorldMinX,
            0.1f,
            redGizmoWorldMaxZ - redGizmoWorldMinZ
        );

        Handles.DrawWireCube(redGizmoCenter, redGizmoSize);
        Handles.Label(redGizmoCenter + Vector3.up * 0.5f, "Outer Wall Pivot Area (Red)");

        // --- Individual Wall Spawn Positions (Yellow) ---
        Handles.color = Color.yellow;
        // Top Walls (+Z direction)
        for (int x = drawStartX; x <= drawEndX; x++)
        {
            Handles.DrawWireCube(GetWallSpawnPosition(x, drawEndY + 1, WallOrientation.AlongXAxis), new Vector3(CELL_SIZE, 0.1f, CELL_SIZE));
        }
        // Bottom Walls (-Z direction)
        for (int x = drawStartX; x <= drawEndX; x++)
        {
            Handles.DrawWireCube(GetWallSpawnPosition(x, drawStartY, WallOrientation.AlongXAxis), new Vector3(CELL_SIZE, 0.1f, CELL_SIZE));
        }
        // Left Walls (-X direction)
        for (int y = drawStartY; y <= drawEndY; y++)
        {
            Handles.DrawWireCube(GetWallSpawnPosition(drawStartX, y, WallOrientation.AlongZAxis), new Vector3(CELL_SIZE, 0.1f, CELL_SIZE));
        }
        // Right Walls (+X direction)
        for (int y = drawStartY; y <= drawEndY; y++)
        {
            Handles.DrawWireCube(GetWallSpawnPosition(drawEndX + 1, y, WallOrientation.AlongZAxis), new Vector3(CELL_SIZE, 0.1f, CELL_SIZE));
        }

        sceneView.Repaint();
    }
}