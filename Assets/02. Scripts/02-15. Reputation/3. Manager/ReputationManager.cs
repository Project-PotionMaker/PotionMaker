//using Photon.Pun;
using System;
//using UnityEngine;
//using PhotonPlayer = Photon.Realtime.Player;

public class ReputationManager : MonoBehaviourPunCallbacksSingleton<ReputationManager>
{
    public event Action OnDataChanged;

    //private PhotonView _photonView;
    //public PhotonView PhotonView => _photonView;

    private Reputation _reputation;
    public ReputationDTO Reputation => _reputation.ToDTO();

    private ReputationRepository _reputationRepository;

    private const float _increaseAmountOnSuccessOrder = 0.01f;
    private const float _decreaseAmountOnFailOrder = 0.2f;

    //protected override void Awake()
    //{
    //    base.Awake();
    //    _photonView = GetComponent<PhotonView>();
    //}

    private void Start()
    {
        InitReputationManager();
    }

    private void InitReputationManager()
    {
        _reputationRepository = new ReputationRepository();
        _reputation = new Reputation(0);
        // Todo: 리포지토리 구현하고 데이터 로드해와야 한다.
        UpdateReputation(_reputation.Value);
        OnDataChanged?.Invoke();
    }

    public void RequestAddReputation(float addedValue = _increaseAmountOnSuccessOrder)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    _photonView.RPC(nameof(RPC_AddReputation), RpcTarget.MasterClient, addedValue);
        //    return;
        //}

        AddReputation(addedValue);
    }

    //[PunRPC]
    public void RPC_AddReputation(float addedValue)
    {
        AddReputation(addedValue);
    }

    private void AddReputation(float addedValue)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    throw new InvalidOperationException
        //        ("마스터 클라이언트만 평판을 증가시킬 수 있습니다.");
        //}
        //_reputation.AddReputation(addedValue);
        //OnDataChanged?.Invoke();

        //_photonView.RPC(nameof(RPC_UpdateReputation), RpcTarget.Others, _reputation.Value);
    }

    public void RequestSubtractReputation(float subtractedValue = _decreaseAmountOnFailOrder)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    _photonView.RPC(nameof(RPC_SubtractReputation), RpcTarget.MasterClient, subtractedValue);
        //    return;
        //}

        SubtractReputation(subtractedValue);
    }

    //[PunRPC]
    public void RPC_SubtractReputation(float subtractedValue)
    {
        SubtractReputation(subtractedValue);
    }

    private void SubtractReputation(float subtractedValue)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    throw new InvalidOperationException
        //        ("마스터 클라이언트만 평판을 감소시킬 수 있습니다.");
        //}

        //bool result = _reputation.TrySubtractReputation(subtractedValue);
        //if (result)
        //{
        //    OnDataChanged?.Invoke();
        //    _photonView.RPC(nameof(RPC_UpdateReputation), RpcTarget.Others, _reputation.Value);
        //    Debug.Log("평판 감소...");
        //    return;
        //}
        //Debug.Log("평판 감소 실패");
    }

    //[PunRPC]
    public void RPC_UpdateReputation(float value)
    {
        UpdateReputation(value);
    }

    private void UpdateReputation(float value)
    {
        _reputation.SetReputation(value);
        OnDataChanged?.Invoke();
    }

    public void OnServingPhaseEnd()
    {
        _reputation.UpdateValueYesterDay();
    }
}