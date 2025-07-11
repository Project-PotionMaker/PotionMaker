using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    //Customer 스폰 및 초기화는 ObjectPooling구현 이후

    [SerializeField]
    private int _lostCustomerCount;
    public int LostCustomerCount { get => _lostCustomerCount; set => _lostCustomerCount = value; }

    private const int MAX_CUSTOMER_LOST = 5; 
    public void AddLostCustomer()
    {
        _lostCustomerCount++;
        if(_lostCustomerCount >= MAX_CUSTOMER_LOST)
        {
            PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
        }
    }
}
