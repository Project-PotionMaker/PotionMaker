using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseFactory<TEnum, TFactoryInfo> : MonoBehaviourSingleton<BaseFactory<TEnum, TFactoryInfo>>
    where TEnum : Enum
    where TFactoryInfo : BaseFactoryInfo<TEnum>
{
    [SerializeField]
    private List<TFactoryInfo> _factoryInfoList;

    private Dictionary<TEnum, GameObject> _typeToPrefabKeyDict = new Dictionary<TEnum, GameObject>();

    private async void Start()
    {
        foreach (TFactoryInfo info in _factoryInfoList)
        {
            if (_typeToPrefabKeyDict.ContainsKey(info.Type))
            {
                Debug.LogError($"Type이 중복되었습니다. Type : {info.Type}");
                continue;
            }

            GameObject prefab = await AssetManager.Instance.LoadAsset<GameObject>(info.AddressableKey);

            if (prefab != null)
            {
                _typeToPrefabKeyDict.TryAdd(info.Type, prefab);

                var identity = prefab.GetComponent<NetworkIdentity>();
                if (identity != null)
                {
                    MirrorNetworkManager.Instance.spawnPrefabs.Add(prefab);
                }
            }
            else
            {
                Debug.LogError($"프리팹이 없습니다. 키: {info.AddressableKey}");
                continue;
            }
        }
    }

    [Server]
    public GameObject Create(TEnum type, Vector3 position, Quaternion rotation)
    {
        if (!_typeToPrefabKeyDict.ContainsKey(type))
        {
            Debug.LogError($"타입에 맞는 프리팹이 없습니다. 타입 : {type}");
            return null;
        }

        // Mirror 추가
        GameObject networkObject = Instantiate(_typeToPrefabKeyDict[type], position, rotation);
        NetworkServer.Spawn(networkObject);

        Debug.Log($"서버에서 '{networkObject.name}'를 스폰했습니다.");
        return networkObject;
    }

    [Server]
    private void Return(GameObject obj)
    {
        NetworkServer.UnSpawn(obj);
        obj.SetActive(false);
    }

    [Command]
    public void CmdReturn(GameObject obj)
    {
        Return(obj);
    }
}
