using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseFactory<TEnum, TFactoryInfo> : MonoBehaviourSingleton<BaseFactory<TEnum, TFactoryInfo>>
    where TEnum : Enum
    where TFactoryInfo : BaseFactoryInfo<TEnum>
{
    [SerializeField]
    private List<TFactoryInfo> _factoryInfoList;

    private Dictionary<TEnum, string> _typeToAddressableKeyMap = new Dictionary<TEnum, string>();
    private HashSet<string> _validAddressableKeys = new HashSet<string>();

    protected override void Awake()
    {
        base.Awake();
    }

    private async void Start()
    {
        DefaultPool defaultPool = PhotonNetwork.PrefabPool as DefaultPool;

        foreach (TFactoryInfo info in _factoryInfoList)
        {
            if (_typeToAddressableKeyMap.ContainsKey(info.Type))
            {
                Debug.LogError($"Type이 중복되었습니다. Type : {info.Type}");
                continue;
            }

            GameObject prefab = await AssetManager.Instance.LoadAsset<GameObject>(info.AddressableKey);

            if (prefab != null)
            {
                _validAddressableKeys.Add(info.AddressableKey);
                _typeToAddressableKeyMap[info.Type] = info.AddressableKey;
                defaultPool.ResourceCache.Add(info.AddressableKey, prefab);
            }
            else
            {
                Debug.LogError($"프리팹이 없습니다. 키: {info.AddressableKey}");
                continue;
            }
        }
    }

    public GameObject Create(TEnum type, Vector3 position, Quaternion rotation)
    {
        if (!_typeToAddressableKeyMap.ContainsKey(type))
        {
            Debug.LogError($"타입에 맞는 어드레서블 키가 없습니다. 타입 : {type}");
            return null;
        }
        string addressableKey = _typeToAddressableKeyMap[type];
        
        // PhotonNetwork.Instantiate는 Addressable Key를 사용해야 하므로 캐시도 이름 기준
        GameObject networkObject = PhotonNetwork.Instantiate(addressableKey, position, rotation);

        return networkObject;
    }

    public GameObject Create(string addressableKey, Vector3 position, Quaternion rotation)
    {
        if (!_validAddressableKeys.Contains(addressableKey))
        {
            Debug.LogError($"Factory에 등록된 어드레서블 키가 없습니다. Addressable Key : {addressableKey}");
            return null;
        }

        GameObject networkObject = PhotonNetwork.Instantiate(addressableKey, position, rotation);

        return networkObject;
    }

    public void Return(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
    }
}
