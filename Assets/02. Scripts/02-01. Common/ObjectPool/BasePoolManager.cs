using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;
using System.Threading.Tasks;
using Unity.VisualScripting;

[RequireComponent(typeof(PhotonView))]
public class BasePoolManager<TEnum, TPoolInfo> : MonoBehaviourSingleton<BasePoolManager<TEnum, TPoolInfo>>
    where TEnum : Enum
    where TPoolInfo : BasePoolInfo<TEnum>
{
    [Header("풀 세팅")]
    [SerializeField]
    private List<TPoolInfo> _poolInfoList;
    [SerializeField]
    private BaseFactory _factory;

    private PhotonView _photonView;

    // PoolList의 타입별 정보 저장
    private Dictionary<TEnum, TPoolInfo> _poolInfoDictionary = new Dictionary<TEnum, TPoolInfo>();

    // 오브젝트 반환 시 콜백을 위한 이벤트
    public Dictionary<TEnum, Action<int>> ObjectSpawnedActions = new Dictionary<TEnum, Action<int>>();

    protected override void Awake()
    {
        base.Awake();
        
        _photonView = GetComponent<PhotonView>();
    }

    //private async void Start()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        await InitializeAsync();
    //    }
    //}

    public async Task InitializeAsync()
    {
        // 딕셔너리 초기화
        foreach (TPoolInfo info in _poolInfoList)
        {
            _poolInfoDictionary[info.Type] = info;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // 초기 오브젝트 생성
            foreach (TPoolInfo info in _poolInfoList)
            {
                await CreateInitialObjects(info);
            }
        }
    }

    private async Task CreateInitialObjects(TPoolInfo info)
    {
        for (int i = 0; i < info.InitCount; i++)
        {
            GameObject newObject = await CreateNewObjectAsync(info);
            if (newObject != null)
            {
                PhotonView targetPhotonView = newObject.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    int typeInt = Convert.ToInt32(info.Type);
                    _photonView.RPC(nameof(AddObjectToPool), RpcTarget.All, targetPhotonView.ViewID, typeInt);
                }
            }
        }
    }

    private async Task<GameObject> CreateNewObjectAsync(TPoolInfo info)
    {
        GameObject newObject = await _factory.RequestCreateAsync(info.AddressableKey);
        if (newObject != null)
        {
            if (info.Container != null)
            {
                newObject.transform.SetParent(info.Container);
            }

            newObject.SetActive(false);
        }

        return newObject;
    }

    [PunRPC]
    public void AddObjectToPool(int viewID, int typeInt)
    {
        TEnum type = (TEnum)Enum.ToObject(typeof(TEnum), typeInt);

        TPoolInfo info = GetPoolByType(type);
        if (info == null) return;

        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView == null) return;

        info.PoolQueue.Enqueue(targetPhotonView.gameObject);
        targetPhotonView.gameObject.SetActive(false);
    }

    private TPoolInfo GetPoolByType(TEnum type)
    {
        if (_poolInfoDictionary.TryGetValue(type, out TPoolInfo info))
        {
            return info;
        }

        return null;
    }

    public void GetObjectAsync(TEnum type)
    {
        int typeInt = Convert.ToInt32(type);
        if (PhotonNetwork.IsMasterClient)
        {
            GetObjectAsyncMaster(typeInt);
        }
        else
        {
            _photonView.RPC(nameof(GetObjectAsyncMaster), RpcTarget.MasterClient, typeInt);
        }
    }

    [PunRPC]
    private async void GetObjectAsyncMaster(int typeInt)
    {
        TEnum type = (TEnum)Enum.ToObject(typeof(TEnum), typeInt);
        TPoolInfo info = GetPoolByType(type);
        if (info == null) return;

        if (info.PoolQueue.Count <= 0)
        {
            GameObject newObject = await CreateNewObjectAsync(info);
            if (newObject != null)
            {
                PhotonView targetPhotonView = newObject.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    // 새로 생성한 오브젝트를 모든 클라이언트의 풀에 추가
                    _photonView.RPC(nameof(GetObjectFromPoolWithViewID), RpcTarget.All, targetPhotonView.ViewID, type);
                    return;
                }
            }
        }

        _photonView.RPC(nameof(GetObjectFromPool), RpcTarget.All, typeInt);
    }

    [PunRPC]
    public void GetObjectFromPool(int typeInt)
    {
        TEnum type = (TEnum)Enum.ToObject(typeof(TEnum), typeInt);
        TPoolInfo info = GetPoolByType(type);
        if (info == null || info.PoolQueue.Count <= 0) return;

        GameObject obj = info.PoolQueue.Dequeue();
        if (obj != null)
        {
            obj.SetActive(true);

            PhotonView targetPhotonView = obj.GetComponent<PhotonView>();

            ObjectSpawnedActions[type]?.Invoke(targetPhotonView.ViewID);
        }
    }

    [PunRPC]
    public void GetObjectFromPoolWithViewID(int viewID, int typeInt)
    {
        TEnum type = (TEnum)Enum.ToObject(typeof(TEnum), typeInt);
        TPoolInfo info = GetPoolByType(type);
        if (info == null) return;

        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView == null) return;

        GameObject obj = targetPhotonView.gameObject;

        if (obj != null)
        {
            obj.SetActive(true);

            ObjectSpawnedActions[type]?.Invoke(targetPhotonView.ViewID);
        }
    }

    public void ReturnObject(GameObject obj, TEnum type)
    {
        PhotonView targePhotonView = obj.GetComponent<PhotonView>();
        if (targePhotonView == null) return;

        int viewID = targePhotonView.ViewID;
        int typeInt = Convert.ToInt32(type);
        if (PhotonNetwork.IsMasterClient)
        {
            ReturnObjectMaster(viewID, typeInt);
        }
        else
        {
            _photonView.RPC(nameof(ReturnObjectMaster), RpcTarget.MasterClient, obj, typeInt);
        }
    }

    [PunRPC]
    public void ReturnObjectMaster(int viewID, int typeInt)
    {
        TEnum type = (TEnum)Enum.ToObject(typeof(TEnum), typeInt);
        _photonView.RPC(nameof(ReturnObjectToPool), RpcTarget.All, viewID, typeInt);
    }

    [PunRPC]
    public void ReturnObjectToPool(int viewID, int typeInt)
    {
        TEnum type = (TEnum)Enum.ToObject(typeof(TEnum), typeInt);
        TPoolInfo info = GetPoolByType(type);
        if (info == null) return;

        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView == null) return;

        info.PoolQueue.Enqueue(targetPhotonView.gameObject);

        targetPhotonView.gameObject.SetActive(false);
    }
}
