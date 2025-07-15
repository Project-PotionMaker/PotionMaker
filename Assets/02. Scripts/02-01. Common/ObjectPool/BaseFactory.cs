using Photon.Pun;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BaseFactory : MonoBehaviourPun, IPunPrefabPool
{
    private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        PhotonNetwork.PrefabPool = this; // 커스텀 풀 등록
    }

    public async Task<GameObject> RequestCreateAsync(string addressableKey)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return await CreateAsync(addressableKey);
        }
        else
        {
            photonView.RPC(nameof(CreateRPC), RpcTarget.MasterClient, addressableKey);
            return null;
        }
    }

    [PunRPC]
    private async void CreateRPC(string addressableKey)
    {
        await CreateAsync(addressableKey);
    }

    private async Task<GameObject> CreateAsync(string addressableKey)
    {
        if (!_prefabCache.ContainsKey(addressableKey))
        {
            GameObject prefab = await AssetManager.Instance.LoadAsset<GameObject>(addressableKey);

            if (prefab != null)
            {
                _prefabCache[addressableKey] = prefab; // 이름 기준으로 캐싱
            }
            else
            {
                Debug.LogError($"프리팹이 없습니다. 키: {addressableKey}");
                return null;
            }
        }
        // PhotonNetwork.Instantiate는 prefab.name을 사용해야 하므로 캐시도 이름 기준
        GameObject networkObject = PhotonNetwork.Instantiate(addressableKey, Vector3.zero, Quaternion.identity);

        return networkObject;
    }

    // Photon이 내부적으로 호출하는 메서드
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (_prefabCache.TryGetValue(prefabId, out GameObject prefab))
        {
            GameObject obj = GameObject.Instantiate(prefab, position, rotation);
            obj.SetActive(false);
            return obj;
        }
        Debug.LogError($"풀에서 프리팹을 찾을 수 없습니다: {prefabId}");
        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }
}
