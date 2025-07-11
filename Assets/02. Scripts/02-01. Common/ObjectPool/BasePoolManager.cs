using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public abstract class BasePoolManager<TEnum, TPoolInfo> : MonoBehaviourSingleton<BasePoolManager<TEnum, TPoolInfo>>, IPunPrefabPool
    where TEnum : Enum
    where TPoolInfo : BasePoolInfo<TEnum>
{
    [Header("풀 세팅")]
    [SerializeField]
    private List<TPoolInfo> _poolInfoList;
    [SerializeField]
    private BaseFactory _factory;

    private PhotonView _photonView;
    // PoolList의 타입별 시작 위치
    private Dictionary<TEnum, int> _startIndexDictionary = new Dictionary<TEnum, int>();


    protected override void Awake()
    {
        base.Awake();
        
        _photonView = GetComponent<PhotonView>();
        Initialize();
    }

    private void Initialize()
    {
        _poolInfoList.Sort((a, b) => a.Type.CompareTo(b.Type));

        int index = 0;

        foreach (TPoolInfo info in _poolInfoList)
        {
            for (int i = 0; i < info.InitCount; i++)
            {
                info.PoolQueue.Enqueue(CreateNewObject(info));
            }

            if (!_startIndexDictionary.ContainsKey(info.Type))
            {
                _startIndexDictionary[info.Type] = index;
            }
            
            index++;
        }
    }

    private GameObject CreateNewObject(TPoolInfo info)
    {
        GameObject newObject = _factory.Create(info.AddressableKey);
        newObject.SetActive(false);
        return newObject;
    }

    private TPoolInfo GetPoolByType(TEnum type)
    {
        if (!_startIndexDictionary.ContainsKey(type))
        {
            return null;
        }

        int startIndex = _startIndexDictionary[type];
        return _poolInfoList[startIndex];
    }

    [PunRPC]
    public GameObject GetObject(TEnum type)
    {
        TPoolInfo info = GetPoolByType(type);
        if (info == null) return null;

        GameObject obj;
        if (info.PoolQueue.Count > 0)
        {
            obj = info.PoolQueue.Dequeue();
        }
        else
        {
            obj = CreateNewObject(info);
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            SetObjectActive(obj, true);
        }
        else
        {
            _photonView.RPC(nameof(SetObjectActive), RpcTarget.MasterClient, true);
        }
        
        return obj;
    }

    public void ReturnObject(GameObject obj, TEnum type)
    {
        TPoolInfo info = GetPoolByType(type);
        if (info == null) return;

        info.PoolQueue.Enqueue(obj);
        
        if (PhotonNetwork.IsMasterClient)
        {
            SetObjectActive(obj, false);
        }
        else
        {
            _photonView.RPC(nameof(SetObjectActive), RpcTarget.MasterClient, false);
        }
    }

    [PunRPC]
    private void SetObjectActive(GameObject target, bool isActive)
    {
        target.SetActive(isActive);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        throw new NotImplementedException();
    }

    public void Destroy(GameObject gameObject)
    {
        throw new NotImplementedException();
    }
}
