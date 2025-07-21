using Photon.Pun;
using System;
using UnityEngine;

public class ReputationManager : MonoBehaviourPunCallbacksSingleton<ReputationManager>
{
    public event Action OnDataChanged;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    private Reputation _reputation;
    public ReputationDTO Reputation => _reputation.ToDTO();

    private ReputationRepository _reputationRepository;

    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitReputationManager();
    }

    private void InitReputationManager()
    {
        _reputationRepository = new ReputationRepository();
        _reputation = new Reputation(0);
        _photonView.RPC(nameof(RPC_RequestSetReputation), RpcTarget.MasterClient);
        // Todo: Save총괄로부터 데이터 받아온 후 초기화
        OnDataChanged?.Invoke();
    }

    [PunRPC]
    public void RPC_RequestAddReputation(int addendValue)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_RequestAddReputation), RpcTarget.MasterClient, addendValue);
            return;
        }

        AddReputation(addendValue);
    }

    private void AddReputation(int addendValue)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException
                ("마스터 클라이언트만 평판을 증가시킬 수 있습니다. 대신 RequestAddReputation을 사용하세요.");
        }
        _reputation.AddReputation(addendValue);
        OnDataChanged?.Invoke();

        _photonView.RPC(nameof(RPC_SetReputation), RpcTarget.Others, _reputation.Value);
    }

    public bool TrySubtractReputation(int subtrahendValue)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException
                ("마스터 클라이언트만 평판을 감소시킬 수 있습니다.");
        }

        bool result = _reputation.TrySubtractReputation(subtrahendValue);
        if (result)
        {
            OnDataChanged?.Invoke();
            _photonView.RPC(nameof(RPC_SetReputation), RpcTarget.Others, _reputation.Value);
            Debug.Log("평판 감소...");
            return true;
        }
        Debug.Log("평판 감소 실패");
        return false;
    }


    [PunRPC]
    public void RPC_RequestSetReputation(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_RequestSetReputation), RpcTarget.MasterClient);
            return;
        }

        _photonView.RPC(nameof(RPC_SetReputation), info.Sender, _reputation.Value);
    }

    [PunRPC]
    public void RPC_SetReputation(int value, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            throw new InvalidOperationException("마스터 클라이언트만 평판을 Setting할 수 있습니다.");
        }

        _reputation.SetReputation(value);
        OnDataChanged?.Invoke();
    }
}