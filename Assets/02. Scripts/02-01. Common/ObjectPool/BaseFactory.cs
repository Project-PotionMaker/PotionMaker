using UnityEngine;
using UnityEngine.AddressableAssets;

public class BaseFactory : MonoBehaviour
{
    public GameObject Create(string addressableKey)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);

        GameObject prefab = handle.WaitForCompletion();

        GameObject instance = Instantiate(prefab);
        return instance;
    }
}
