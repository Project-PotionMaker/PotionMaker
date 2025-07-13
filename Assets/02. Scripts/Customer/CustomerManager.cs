using UnityEngine;
using System.Collections.Generic;

public class CustomerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _customerPrefab;
    public GameObject CustomerPrefab { get { return _customerPrefab; } }
    private const int POOL_SIZE = 10;

    private List<GameObject> _customerPool;
    private List<GameObject> _customerInHall;
    private Queue<GameObject> _customerInLine;

    [SerializeField]
    private int _lostCustomerCount;
    public int LostCustomerCount { get => _lostCustomerCount; set => _lostCustomerCount = value; }

    private const int MAX_CUSTOMER_LOST = 5;

    private void Start()
    {
        _customerInHall = new List<GameObject>();
        _customerInLine = new Queue<GameObject>();
        _customerPool = new List<GameObject>();
        for (int i = 0; i < MAX_CUSTOMER_LOST; i++)
        {
            _customerPool.Add(new GameObject());
        }
        _lostCustomerCount = 0;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += SetLists;
    }

    private void SetLists()
    {
        foreach (GameObject customer in _customerPool) 
        {
            customer.SetActive(false);
        }
        _customerInHall.Clear();
        _customerInLine.Clear();
    }
    public void AddLostCustomer(GameObject customer)
    {
        ReturnCustomer(customer);
        _lostCustomerCount++;
        if(_lostCustomerCount >= MAX_CUSTOMER_LOST)
        {
            PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
        }
    }

    public void ReturnCustomerFromHall(GameObject customer)
    {
        //TODO : 접수처에 와서 포션 받아가기
        _customerInHall.Remove(customer);
        ReturnCustomer(customer);
    }

    public void ReturnAllCustomerFromLine()
    {
        while(_customerInLine != null) 
        {
            ReturnCustomer(_customerInLine.Peek());
            _customerInLine.Dequeue();
        }
    }

    public void ReturnCustomer(GameObject customer)
    {
        //TODO : 손님 오브젝트를 건물 밖으로 내보내기
        // 건물 밖에 나가면
        customer.SetActive(false);
    }
}
