using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviourSingleton<AddressableManager>
{
    // 로드된 프리팹 캐시
    private readonly Dictionary<string, GameObject> prefabCache = new();
    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> handleCache = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public async Task<GameObject> LoadPrefabAsync(string address)
    {
        if (prefabCache.TryGetValue(address, out var cached))
        {
            return cached;
        }

        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;
            prefabCache[address] = prefab;
            handleCache[address] = handle;
            return prefab;
        }
        else
        {
            Debug.LogError($"[AddressableManager] 다음 주소에 대해 프리팹 로드를 실패했습니다: {address}");
            return null;
        }
    }

    // 특정 프리팹에 대한 Addressable 리소스를 반환(해제)
    public void ReleasePrefab(string address)
    {
        if (handleCache.TryGetValue(address, out var handle))
        {
            Addressables.Release(handle);
            handleCache.Remove(address);
            prefabCache.Remove(address);
        }
    }

    // 전체 캐시된 프리팹 리소스를 반환
    public void ReleaseAll()
    {
        foreach (var handle in handleCache.Values)
        {
            Addressables.Release(handle);
        }
        handleCache.Clear();
        prefabCache.Clear();
    }
}
