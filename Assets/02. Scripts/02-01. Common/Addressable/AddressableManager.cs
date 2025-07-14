using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine;
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
}
