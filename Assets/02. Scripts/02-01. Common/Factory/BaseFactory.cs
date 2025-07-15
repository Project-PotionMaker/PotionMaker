using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class BaseFactory<TEnum, TFactoryInfo> : MonoBehaviourSingleton<BaseFactory<TEnum, TFactoryInfo>>, IPunPrefabPool
    where TEnum : Enum
    where TFactoryInfo : BaseFactoryInfo<TEnum>
{
    [SerializeField]
    private List<TFactoryInfo> _factoryInfoList;

    // private PhotonView _photonView;

    private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();
    private Dictionary<TEnum, string> _addressableKeyCache = new Dictionary<TEnum, string>();

    protected override void Awake()
    {
        base.Awake();

       // _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        PhotonNetwork.PrefabPool = this;
        foreach (TFactoryInfo info in _factoryInfoList)
        {
            if (_addressableKeyCache.ContainsKey(info.Type))
            {
                Debug.LogError($"Type이 중복되었습니다. Type : {info.Type}");
                continue;
            }

            _addressableKeyCache[info.Type] = info.AddressableKey;
        }
    }

    public async Task<GameObject> CreateAsync(TEnum type, Vector3 position, Quaternion rotation)
    {
        if (!_addressableKeyCache.ContainsKey(type))
        {
            Debug.LogError($"타입에 맞는 어드레서블 키가 없습니다. 타입 : {type}");
            return null;
        }
        string addressableKey = _addressableKeyCache[type];

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
        GameObject networkObject = PhotonNetwork.Instantiate(addressableKey, position, rotation);

        return networkObject;
    }

    public void Return(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
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
