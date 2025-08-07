using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;

public class AssetManager : MonoBehaviourSingleton<AssetManager>
{
    // GameObject 타입만 캐싱
    private Dictionary<string, GameObject> _prefabCacheDict = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    public async Task<T> LoadAsset<T>(string key) where T : Object
    {
        if (typeof(T) == typeof(GameObject) && _prefabCacheDict.TryGetValue(key, out var cachedPrefab))
        {
            return cachedPrefab as T;
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        T asset = await handle.Task;

        if (asset != null)
        {
            if (typeof(T) == typeof(GameObject))
            {
                _prefabCacheDict[key] = asset as GameObject;
            }
            Debug.Log($"로드된 에셋명 : {asset.name}");
            return asset;
        }
        else
        {
            Debug.LogWarning($"에셋 로드 실패 : {key}");
            return null;
        }
    }

    public async Task<List<T>> LoadAssetsByLabel<T>(string label) where T : Object
    {
        List<T> result = new List<T>();

        AsyncOperationHandle<IList<IResourceLocation>> keyListHandle = Addressables.LoadResourceLocationsAsync(label);
        await keyListHandle.Task;

        if (keyListHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning($"[{label}] 라벨의 주소 리스트 로드 실패");
            return null;
        }

        foreach (var location in keyListHandle.Result)
        {
            string key = location.PrimaryKey;
            if (typeof(T) == typeof(GameObject) && _prefabCacheDict.TryGetValue(key, out var cachedPrefab))
            {
                result.Add(cachedPrefab as T);
                continue;
            }

            T asset = await LoadAsset<T>(key);
            if (asset != null)
            {
                result.Add(asset);
            }
            else
            {
                Debug.LogWarning($"에셋 로드 실패 : {key}");
            }
        }
        return result;
    }

    public void UnloadCachedPrefab(string key)
    {
        if (_prefabCacheDict.TryGetValue(key, out var go))
        {
            Addressables.Release(go);
            _prefabCacheDict.Remove(key);
            Debug.Log($"캐시 해제 : {key}");
        }
    }

    public void ClearAllCachedPrefabs()
    {
        foreach (var pair in _prefabCacheDict)
        {
            Addressables.Release(pair.Value);
        }
        _prefabCacheDict.Clear();
        Debug.Log("모든 캐시 해제");
    }
}
