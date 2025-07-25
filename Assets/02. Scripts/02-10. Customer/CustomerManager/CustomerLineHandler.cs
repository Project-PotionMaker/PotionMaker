using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class CustomerLineHandler // 접수대 앞에 물리적으로 줄 세우는 컴포넌트
{
    private Vector3 _spacing = new Vector3(1.5f, 0, 0); // 손님 줄 사이의 간격, 임시값

    public void ReLining() // 앞 손님 빠지면 줄 다시 세우기
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 호출 가능
        }
        Queue<Customer> line = CustomerManager.Instance.OrderHandler.PotionOrderLine;
        Customer[] lineArray = line.ToArray();
        if (line == null || line.Count == 0)
        {
            return;
        }
        for (int i = 0; i < lineArray.Length; i++)
        {
            Customer customer = lineArray[i];
            customer.CustomerMove.MoveTo(GetLinePosition(i));
        }
    }

    public void PutOutCustomer(Customer customer) // 손님 나가게 하기
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 호출 가능
        }
        customer.CustomerMove.MoveTo(CustomerManager.Instance.ExitDoor.position);
    }

    private Vector3 GetLinePosition(int index)
    {
        return CustomerManager.Instance.CasherLocation.position + (index * _spacing);
    }

    public void ResetLocation()
    {
        //TODO : 이사 가면 타겟 위치들 다시 초기화하기
    }
}
