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

    public async void Initialize(List<TFactoryInfo> factoryInfoList)
    {
        foreach (TFactoryInfo info in factoryInfoList)
        {
            if (_typeToPrefabDict.ContainsKey(info.Type))
            {
                Debug.LogError($"Type이 중복되었습니다. Type : {info.Type}");
                continue;
            }

            GameObject prefab = await AssetManager.Instance.LoadAsset<GameObject>(info.AddressableKey);

            if (prefab != null)
            {
                _typeToPrefabDict.TryAdd(info.Type, prefab);

                var identity = prefab.GetComponent<NetworkIdentity>();
                if (identity != null)
                {
                    MirrorNetworkManager.Instance.spawnPrefabs.Add(prefab);
                }

                InitializePool(info.Type, prefab, 10);
            }
            else
            {
                Debug.LogError($"프리팹이 없습니다. 키: {info.AddressableKey}");
                continue;
            }
        }
    }

    private void InitializePool(TEnum type, GameObject prefab, int poolSize)
    {
        if (_typeToPoolDict.ContainsKey(type))
        {
            Debug.Log($"Type이 중복되었습니다. Type : {type}");
            return;
        }

        _typeToPoolDict[type] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolObject = UnityEngine.Object.Instantiate(prefab);
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

        if (_typeToPoolDict[type].Count <= 0)
        {
            GameObject newObj = UnityEngine.Object.Instantiate(_typeToPrefabDict[type]);
            newObj.SetActive(false);
            _typeToPoolDict[type].Enqueue(newObj);
        }

        // Mirror 추가
        GameObject networkObject = _typeToPoolDict[type].Dequeue();
        networkObject.SetActive(true);
        networkObject.transform.position = position;
        networkObject.transform.rotation = rotation;

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
        obj.SetActive(false);
        if (_typeToPoolDict.ContainsKey(type))
        {
            _typeToPoolDict[type].Enqueue(obj);
        }
    }
}
