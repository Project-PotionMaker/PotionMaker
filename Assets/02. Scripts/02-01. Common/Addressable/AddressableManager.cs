using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviourSingleton<AddressableManager>
{
    // �ε�� ������ ĳ��
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
            Debug.LogError($"[AddressableManager] ���� �ּҿ� ���� ������ �ε带 �����߽��ϴ�: {address}");
            return null;
        }
    }

    // Ư�� �����տ� ���� Addressable ���ҽ��� ��ȯ(����)
    public void ReleasePrefab(string address)
    {
        if (handleCache.TryGetValue(address, out var handle))
        {
            Addressables.Release(handle);
            handleCache.Remove(address);
            prefabCache.Remove(address);
        }
    }

    // ��ü ĳ�õ� ������ ���ҽ��� ��ȯ
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
