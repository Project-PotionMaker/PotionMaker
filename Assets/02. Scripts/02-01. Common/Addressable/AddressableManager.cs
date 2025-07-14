using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AddressableManager : MonoBehaviourSingleton<AddressableManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public async Task<GameObject> LoadPrefab(string addressableKey)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        GameObject prefab = await handle.Task;
        Debug.Log($"원격 저장소에서 로드한 프리팹명 : {prefab.name}");
        return prefab;
    }

    public async Task<List<GameObject>> LoadPrefabsByFolder(string folderKey)
    {
        List<GameObject> result = new List<GameObject>();
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(folderKey, null);
        IList<GameObject> prefabs = await handle.Task;

        foreach (var prefab in prefabs)
        {
            Debug.Log($"업로드된 폴더[{folderKey}]에서 로드한 프리팹명 : {prefab.name}");
            result.Add(prefab);
        }
        return result;
    }

    public async Task<List<GameObject>> LoadPrefabsByLabel(string label)
    {
        List<GameObject> result = new List<GameObject>();
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(label, null);
        IList<GameObject> prefabs = await handle.Task;

        foreach (var prefab in prefabs)
        {
            Debug.Log($"라벨[{label}]로 로드한 프리팹명 : {prefab.name}");
            result.Add(prefab);
        }
        return result;
    }
}
