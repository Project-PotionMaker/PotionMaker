using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryLogic<TEnum, TFactoryInfo>
    where TEnum : Enum
    where TFactoryInfo : BaseFactoryInfo<TEnum>
{
    private Dictionary<TEnum, GameObject> _typeToPrefabDict = new Dictionary<TEnum, GameObject>();

    private Dictionary<TEnum, Queue<GameObject>> _typeToPoolDict = new Dictionary<TEnum, Queue<GameObject>>();

    private Dictionary<GameObject, TEnum> _objectToTypeDict = new Dictionary<GameObject, TEnum>();

    public async void Initialize(List<TFactoryInfo> factoryInfoList, Transform parent)
    {
        foreach (TFactoryInfo info in factoryInfoList)
        {
            if (_typeToPrefabDict.ContainsKey(info.Type))
            {
                Debug.LogError($"Type이 중복되었습니다. Type : {info.Type}");
                continue;
            }

            GameObject prefab = await AssetManager.Instance.LoadAsset<GameObject>(info.AddressableKey);

            if (prefab == null)
            {
                Debug.LogError($"프리팹이 없습니다. 키: {info.AddressableKey}");
                continue;
            }
            
            _typeToPrefabDict.TryAdd(info.Type, prefab);

            var identity = prefab.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                if (!MirrorNetworkManager.Instance.spawnPrefabs.Contains(prefab))
                {
                    MirrorNetworkManager.Instance.spawnPrefabs.Add(prefab);
                }

                if (!NetworkClient.prefabs.ContainsKey(identity.assetId))
                {
                    NetworkClient.RegisterPrefab(
                        prefab,
                        spawnHandler: (position, assetId) => GetObject(info.Type, position, Quaternion.identity),
                        unspawnHandler: obj => ReturnObject(obj)
                    );
                }
            }

            InitializePool(info.Type, prefab, 10, parent);
        }
    }

    private void InitializePool(TEnum type, GameObject prefab, int poolSize, Transform parent)
    {
        if (_typeToPoolDict.ContainsKey(type))
        {
            Debug.Log($"Type이 중복되었습니다. Type : {type}");
            return;
        }

        _typeToPoolDict[type] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolObject = UnityEngine.Object.Instantiate(prefab, parent);

            var identity = poolObject.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                identity.enabled = false;
            }

            poolObject.SetActive(false);
            _typeToPoolDict[type].Enqueue(poolObject);
        }
    }

    public GameObject GetObject(TEnum type, Vector3 position, Quaternion rotation)
    {
        if (!_typeToPrefabDict.ContainsKey(type))
        {
            Debug.LogError($"타입에 맞는 프리팹이 없습니다. 타입 : {type}");
            return null;
        }

        GameObject networkObject;

        // 풀이 비어있으면 새 오브젝트 생성
        if (_typeToPoolDict[type].Count <= 0)
        {
            networkObject = UnityEngine.Object.Instantiate(_typeToPrefabDict[type]);
        }
        else
        {
            networkObject = _typeToPoolDict[type].Dequeue();
        }

        var identity = networkObject.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            identity.enabled = true;
        }

        networkObject.transform.SetPositionAndRotation(position, rotation);
        networkObject.SetActive(true);

        _objectToTypeDict[networkObject] = type;

        return networkObject;
    }

    public void ReturnObject(GameObject obj)
    {
        if (_objectToTypeDict.TryGetValue(obj, out TEnum type))
        {
            ReturnObject(obj, type);
            _objectToTypeDict.Remove(obj);
        }
        else
        {
            Debug.LogError("오브젝트의 타입을 찾을 수 없습니다.");
        }
    }

    public void ReturnObject(GameObject obj, TEnum type)
    {

        var identity = obj.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            identity.enabled = false;
        }

        obj.SetActive(false);
        if (_typeToPoolDict.ContainsKey(type))
        {
            _typeToPoolDict[type].Enqueue(obj);
        }
    }
}
