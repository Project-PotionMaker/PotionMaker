using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class CustomerLineHandler // 접수대 앞에 물리적으로 줄 세우는 컴포넌트
{
    private Vector3 _spacing = new Vector3(0, 0, -1.0f); // 손님 줄 사이의 간격, 임시값

    public void ReLining() // 앞 손님 빠지면 줄 다시 세우기
    {
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
        customer.TransitionState(ECustomerStateType.Leaving);
        customer.CustomerMove.MoveTo(CustomerManager.Instance.ExitDoor.position);
    }

    private Vector3 GetLinePosition(int index)
    {
        return CustomerManager.Instance.CasherLocation.position + ((index+1) * _spacing);
    }
}
