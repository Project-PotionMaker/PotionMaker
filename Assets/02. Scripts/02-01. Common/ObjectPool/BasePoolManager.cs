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

    protected override void Awake()
    {
        base.Awake();
        
        _photonView = GetComponent<PhotonView>();
    }

    private async void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            await InitializeAsync();
        }
    }

    private async Task InitializeAsync()
    {
        // 딕셔너리 초기화
        foreach (TPoolInfo info in _poolInfoList)
        {
            _poolInfoDictionary[info.Type] = info;
        }

        // 초기 오브젝트 생성
        foreach (TPoolInfo info in _poolInfoList)
        {
            await CreateInitialObjects(info);
        }
    }

    private async Task CreateInitialObjects(TPoolInfo info)
    {
        for (int i = 0; i < info.InitCount; i++)
        {
            GameObject newObject = await CreateNewObjectAsync(info);
            if (newObject != null)
            {
                info.PoolQueue.Enqueue(newObject);
            }
        }
    }

    private async Task<GameObject> CreateNewObjectAsync(TPoolInfo info)
    {
        GameObject newObject = await _factory.RequestCreateAsync(info.AddressableKey);
        if (newObject != null)
        {
            newObject.SetActive(false);
            if (info.Container != null)
            {
                newObject.transform.SetParent(info.Container);
            }
        }
        return newObject;
    }



    private TPoolInfo GetPoolByType(TEnum type)
    {
        if (_poolInfoDictionary.TryGetValue(type, out TPoolInfo info))
        {
            return info;
        }

        return null;
    }

    public async Task<GameObject> GetObjectAsync(TEnum type)
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
            obj = await CreateNewObjectAsync(info);
        }

        PhotonView targetPhotonView = obj.GetComponent<PhotonView>();
        if (targetPhotonView == null) return null;

        int targetViewID = targetPhotonView.ViewID;
        
        if (PhotonNetwork.IsMasterClient)
        {
            OnObjectActivated(targetViewID);
        }
        else
        {
            _photonView.RPC(nameof(OnObjectActivated), RpcTarget.MasterClient, targetViewID);
        }
        
        return obj;
    }

    [PunRPC]
    private void OnObjectActivated(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            pv.gameObject.SetActive(true);
        }
    }

    public void ReturnObject(GameObject obj, TEnum type)
    {
        TPoolInfo info = GetPoolByType(type);
        if (info == null) return;

        info.PoolQueue.Enqueue(obj);

        PhotonView targetPhotonView = obj.GetComponent<PhotonView>();
        if (targetPhotonView == null) return;

        int targetViewID = targetPhotonView.ViewID;

        if (PhotonNetwork.IsMasterClient)
        {
            OnObjectDeactivated(targetViewID);
        }
        else
        {
            _photonView.RPC(nameof(OnObjectDeactivated), RpcTarget.MasterClient, targetViewID);
        }
    }

    [PunRPC]
    private void OnObjectDeactivated(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            pv.gameObject.SetActive(false);
        }
    }
}
