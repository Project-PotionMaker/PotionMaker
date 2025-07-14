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

    public async Task<T> LoadAsset<T>(string key) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        T asset = await handle.Task;

        Debug.Log($"로드된 에셋명: {asset.name}");
        return asset;
    } 
    public async Task<List<T>> LoadAssetsByLabel<T>(string label) where T : Object
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
        IList<T> assets = await handle.Task;
        List<T> result = new List<T>(assets);

        foreach (var asset in assets)
        {
            Debug.Log($"라벨[{label}] 기반으로 로드된 에셋명: {asset.name}");
        }
        return result;
    }
}
