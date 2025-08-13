using UnityEditor;
using UnityEngine;

public class BatchRenameEditor : EditorWindow
{
    // 입력 필드를 위한 변수들
    private string prefix = "";
    private string suffix = "";
    private string searchString = "";
    private string replaceString = "";

    // 에디터 윈도우를 메뉴에 추가하고 실행하는 함수
    [MenuItem("Tools/Batch Rename Selected Assets")]
    public static void ShowWindow()
    {
        // 기존에 열려있는 윈도우를 가져오거나, 없다면 새로 생성합니다.
        GetWindow<BatchRenameEditor>("Batch Rename");
    }

    // 윈도우의 GUI를 그리는 함수
    private void OnGUI()
    {
        // --- 1. 접두사/접미사 추가 기능 ---
        GUILayout.Label("Add Prefix / Suffix", EditorStyles.boldLabel);
        prefix = EditorGUILayout.TextField("Prefix (접두사)", prefix);
        suffix = EditorGUILayout.TextField("Suffix (접미사)", suffix);

        if (GUILayout.Button("Add Prefix/Suffix"))
        {
            AddPrefixSuffix();
        }

        // 구분선
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // --- 2. 문자열 찾기 및 바꾸기 기능 ---
        GUILayout.Label("Find and Replace String", EditorStyles.boldLabel);
        searchString = EditorGUILayout.TextField("String to Find (찾을 문자열)", searchString);
        replaceString = EditorGUILayout.TextField("Replace with (바꿀 문자열)", replaceString);

        // 특정 문자열을 제거하고 싶다면 'Replace with' 필드를 비워두면 됩니다.
        EditorGUILayout.HelpBox("To remove a string, leave 'Replace with' empty.", MessageType.Info);

        if (GUILayout.Button("Find and Replace"))
        {
            FindAndReplace();
        }
    }

    // 접두사/접미사를 추가하는 로직
    private void AddPrefixSuffix()
    {
        if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(suffix))
        {
            Debug.LogWarning("접두사와 접미사가 모두 비어있습니다.");
            return;
        }

        // Project 창에서 선택된 모든 '오브젝트'를 가져옵니다.
        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        int renamedCount = 0;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            string newName = prefix + obj.name + suffix;

            // AssetDatabase를 통해 에셋의 이름을 변경합니다.
            string result = AssetDatabase.RenameAsset(assetPath, newName);

            if (string.IsNullOrEmpty(result))
            {
                renamedCount++;
            }
            else
            {
                Debug.LogError($"이름 변경 실패: {obj.name} -> {newName}. 원인: {result}");
            }
        }
        Debug.Log($"{renamedCount}개 에셋의 이름에 접두사/접미사를 추가했습니다.");
    }

    // 문자열을 찾아 바꾸는 로직
    private void FindAndReplace()
    {
        if (string.IsNullOrEmpty(searchString))
        {
            Debug.LogWarning("찾을 문자열이 비어있습니다.");
            return;
        }

        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        int renamedCount = 0;

        foreach (Object obj in selectedObjects)
        {
            // 찾을 문자열이 이름에 포함되어 있지 않으면 건너뜁니다.
            if (!obj.name.Contains(searchString))
            {
                continue;
            }

            string assetPath = AssetDatabase.GetAssetPath(obj);
            string newName = obj.name.Replace(searchString, replaceString);

            string result = AssetDatabase.RenameAsset(assetPath, newName);

            if (string.IsNullOrEmpty(result))
            {
                renamedCount++;
            }
            else
            {
                Debug.LogError($"이름 변경 실패: {obj.name} -> {newName}. 원인: {result}");
            }
        }
        Debug.Log($"{renamedCount}개 에셋의 이름에서 '{searchString}'을(를) '{replaceString}'(으)로 변경했습니다.");
    }
}
