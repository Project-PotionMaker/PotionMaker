using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderHandler
{
    private Dictionary<int, LinkedList<Customer>> _potionOrderMap; // 주문표, 중간 삭제가 가능한 큐
    public Dictionary<int, LinkedList<Customer>> PotionOrderMap { get => _potionOrderMap; set => _potionOrderMap = value; }
    private Queue<Customer> _potionOrderLine; // 손님이 줄을 서는 대기열
    public Queue<Customer> PotionOrderLine { get => _potionOrderLine; set => _potionOrderLine = value; }

    public void Init()
    {
        _potionOrderMap = new Dictionary<int, LinkedList<Customer>>();
        _potionOrderLine = new Queue<Customer>();
    }
    public void SetLists()
    {
        _potionOrderMap.Clear();
        _potionOrderLine.Clear();
    }

    public void AddOrder(int potionTID, Customer customer)
    {
        if (!_potionOrderMap.ContainsKey(potionTID))
        {
            _potionOrderMap[potionTID] = new LinkedList<Customer>();
        }
        _potionOrderMap[potionTID].AddLast(customer);
    }

    public bool RemoveAnywhere(Customer customer)
    {
        if(_potionOrderLine.Contains(customer))
        {
            _potionOrderLine.Dequeue(); // 손님이 줄에 있다면 줄에서 제거
            return true;
        }
        else
        {
            _potionOrderMap[customer.RequestedPotionTID].Remove(customer); // 손님이 홀에 있다면 포션 큐에서 제거
            return false;
        }
    }

}
