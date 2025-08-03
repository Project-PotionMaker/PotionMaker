using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderHandler
{
    private Dictionary<int, LinkedList<Customer>> _potionOrderMap; // 주문표, 중간 삭제가 가능한 큐
    public Dictionary<int, LinkedList<Customer>> PotionOrderMap { get => _potionOrderMap; set => _potionOrderMap = value; }
    private Queue<Customer> _potionOrderLine; // 손님이 줄을 서는 대기열
    public Queue<Customer> PotionOrderLine { get => _potionOrderLine; set => _potionOrderLine = value; }

    private Dictionary<uint, FurnitureUsingStat> _pickupTableDict; // 포션 찾으러 가는 손님들 (제작 완료 번호표)
    public Dictionary<uint, FurnitureUsingStat> PickupTableDict { get => _pickupTableDict; set => _pickupTableDict = value; }
    private Dictionary<uint, FurnitureUsingStat> _oldChairDict;
    public Dictionary<uint, FurnitureUsingStat> OldChairDict { get => _oldChairDict; set => _oldChairDict = value; } 
    private Dictionary<uint, FurnitureUsingStat> _luxuryChairDict;
    public Dictionary<uint, FurnitureUsingStat> LuxuryChairDict { get => _luxuryChairDict; set => _luxuryChairDict = value; }

    public void Init()
    {
        _potionOrderMap = new Dictionary<int, LinkedList<Customer>>();
        _potionOrderLine = new Queue<Customer>();
        _pickupTableDict = new Dictionary<uint, FurnitureUsingStat>();
        _oldChairDict = new Dictionary<uint, FurnitureUsingStat>();
        _luxuryChairDict = new Dictionary<uint, FurnitureUsingStat>();
    }
    public void SetLists()
    {
        _potionOrderMap.Clear();
        _potionOrderLine.Clear();
        _pickupTableDict.Clear();
        _oldChairDict.Clear();
        _luxuryChairDict.Clear();
        foreach (GameObject pickupTable in GridManager.Instance.PickUpTableList)
        {
            _pickupTableDict.Add(pickupTable.GetComponent<NetworkIdentity>().netId, new FurnitureUsingStat()); // 각 픽업 테이블에 대해 초기화
        }
        foreach (GameObject oldChair in GridManager.Instance.OldChairList)
        {
            _oldChairDict.Add(oldChair.GetComponent<NetworkIdentity>().netId, new FurnitureUsingStat()); // 각 오래된 의자에 대해 초기화
        }
        foreach (GameObject luxuryChair in GridManager.Instance.LuxuryChairList)
        {
            _luxuryChairDict.Add(luxuryChair.GetComponent<NetworkIdentity>().netId, new FurnitureUsingStat()); // 각 고급 의자에 대해 초기화
        }
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
    public Customer FindPicker(int potionTID)
    {
        if (!_potionOrderMap.ContainsKey(potionTID) || _potionOrderMap[potionTID].Count == 0)
        {
            return null; // 해당 포션 TID에 대한 주문이 없으면 null 반환
        }

        float minEndurance = float.MaxValue;
        Customer picker = null; // Picking 상태인 손님을 찾기 위한 변수

        foreach (var customer in _potionOrderMap[potionTID])
        {
            if(customer.CustomerEndurance.CurrentEndurance < minEndurance)
            {
                minEndurance = customer.CustomerEndurance.CurrentEndurance; // 최소 인내심을 가진 손님 찾기
                picker = customer; // Picking 상태인 손님을 찾기 위한 변수
            }
        }
        
        return picker;
    }
    public uint FindAvailableChair()
    {
        // 1. 고급 의자 중 비어있는 것들 수집
        List<uint> availableLuxury = new List<uint>();
        foreach (var pair in _luxuryChairDict)
        {
            if (!pair.Value.IsUsing)
            {
                availableLuxury.Add(pair.Key);
            }
        }

        if (availableLuxury.Count > 0)
        {
            int index = (int)Random.Range(0, availableLuxury.Count);
            return availableLuxury[index];
        }

        // 2. 허름한 의자 중 비어있는 것들 수집
        List<uint> availableOld = new List<uint>();
        foreach (var pair in _oldChairDict)
        {
            if (!pair.Value.IsUsing)
            {
                availableOld.Add(pair.Key);
            }
        }

        if (availableOld.Count > 0)
        {
            int index = Random.Range(0, availableOld.Count);
            return availableOld[index];
        }

        // 3. 모두 사용 중이면 null 반환
        return 0;
    }

    public FurnitureUsingStat FindUsingChair(Customer customer)
    {
        foreach (var pair in _oldChairDict)
        {
            if (pair.Value.IsUsing && pair.Value.UsingCustomer == customer)
            {
                return pair.Value; 
            }
        }
        foreach (var pair in _luxuryChairDict)
        {
            if (pair.Value.IsUsing && pair.Value.UsingCustomer == customer)
            {
                return pair.Value; 
            }
        }
        return null; // 사용 중인 의자가 없으면 0 반환
    }


}
