using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class Customer : MonoBehaviour
{
    private int _requestedPotionTID;
    public int RequestedPotionTID { get => _requestedPotionTID; set=> _requestedPotionTID = value; } // 요청한 포션 ID

    private PhotonView _photonView;

    private Vector3 _lastTarget;

    //TODO : 인내심 타이머

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        Waiting();
    }

    private void Waiting()
    {
        //TODO : if 홀에 있다면
        //TODO : 인내심 타이머 감소
        //TODO : 정해진 구역 내에서 랜덤하게 움직이기
        //TODO : 빈 의자가 있으면 가서 앉기
        //TODO : if 줄에 있다면
        //TODO : 인내심만 줄고 아무것도 안함
    }

    public void MoveTo(Vector3 target)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _photonView.RPC(nameof(RPC_MoveTo), RpcTarget.All, target);

    }
    [PunRPC]
    public void RPC_MoveTo(Vector3 target)
    {
        //TODO : target의 Transform.position으로 천천히 이동
        // 정확히 도착 안 해도 근처 가면 OnArrived() 호출되게 하기
        _lastTarget = target;
    }
    public void OnArrived()
    {
        //TODO : 이동이 끝났을 때 호출
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 호출 가능
        }

        if (_lastTarget == CustomerManager.Instance.CounterLocation.position)
        {
            CustomerManager.Instance.OnArrivedLine(this); // 손님이 줄에 도착했을 때 호출
        }
        else if (_lastTarget == CustomerManager.Instance.ServingCounter.position) 
        {
            CustomerManager.Instance.OnServedSuccess(_requestedPotionTID);// 손님이 포션 제공대에 도착했을 때 호출
        }
        else if (_lastTarget == CustomerManager.Instance.ExitDoor.position)
        {
            CustomerManager.Instance.ReturnCustomer(this); // 손님이 나가는 문에 도착했을 때 호출
        }

    }
}
