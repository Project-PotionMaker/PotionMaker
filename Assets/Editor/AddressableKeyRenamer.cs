using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets; // Addressables 에디터 기능을 사용하기 위해 필수
using UnityEditor.AddressableAssets.Settings; // Addressables 설정에 접근하기 위해 필수
using System.Collections.Generic;

public class AddressableKeyRenamer : EditorWindow
{
    private string prefix = "Prefab_";
    private int startNumber = 0;

    // 에디터 윈도우를 메뉴에 추가하고 실행하는 함수
    [MenuItem("Tools/Batch Rename Addressable Keys")]
    public static void ShowWindow()
    {
        // 윈도우 생성 및 타이틀 설정
        GetWindow<AddressableKeyRenamer>("Addressable Key Rename");
    }

    // 윈도우의 GUI를 그리는 함수
    private void OnGUI()
    {
        GUILayout.Label("Rename Addressable Keys Sequentially", EditorStyles.boldLabel);

        // 사용자 입력을 받을 필드
        prefix = EditorGUILayout.TextField("접두사 (Prefix)", prefix);
        startNumber = EditorGUILayout.IntField("시작 번호 (Start Number)", startNumber);

        EditorGUILayout.Space();

        // 도움말 박스 추가
        EditorGUILayout.HelpBox("Project 창에서 에셋을 선택한 후 아래 버튼을 누르세요. 선택 순서에 따라 번호가 매겨집니다.", MessageType.Info);

        EditorGUILayout.Space();

        // 버튼을 누르면 이름 변경 로직 실행
        if (GUILayout.Button("Execute Rename"))
        {
            RenameAddressableKeys();
        }
    }

    private void RenameAddressableKeys()
    {
        // 어드레서블 에셋 설정 가져오기
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("어드레서블 설정 파일을 찾을 수 없습니다. Window > Asset Management > Addressables > Groups 메뉴를 열어 설정을 초기화해주세요.");
            return;
        }

        // Project 창에서 선택된 에셋들을 가져옴
        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Project 창에서 에셋을 먼저 선택해주세요.");
            return;
        }

        int currentNumber = startNumber;
        int renamedCount = 0;

        // 선택된 각 에셋에 대해 작업 수행
        foreach (Object obj in selectedObjects)
        {
            // 에셋의 경로를 통해 GUID(고유 식별자)를 얻음
            string path = AssetDatabase.GetAssetPath(obj);
            string guid = AssetDatabase.AssetPathToGUID(path);

            // GUID를 사용하여 어드레서블 엔트리를 찾음
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);

            if (entry != null)
            {
                // 새 어드레서블 키(주소) 생성: 접두사 + 번호
                string newAddress = $"{prefix}{currentNumber}";

                // 어드레서블 주소 변경
                entry.SetAddress(newAddress);

                renamedCount++;
                currentNumber++; // 다음 번호로 증가
            }
        }

        // 변경된 내용을 설정에 반영하고 저장
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, null, true);

        Debug.Log($"{renamedCount}개의 어드레서블 키를 성공적으로 변경했습니다.");
    }
}