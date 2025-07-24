using Photon.Pun;
using UnityEngine;

public class PickUpTableOutputContainer : IOutputContainer<Furniture, FurnitureStat>
{
    private GameObject _output;

    public GameObject TakeItem(Furniture furniture, FurnitureStat stat)
    {
        GameObject output = stat.InputObject;
        stat.InputObject = null;
        return output;
    }

    public bool CanTake(Furniture furniture, FurnitureStat stat)
    {
        if(stat.InputObject == null)
        {
            return false;
        }
        if (CustomerManager.Instance.OrderHandler.PickingCustomers[furniture] != null)
        {
            Debug.Log("이미 가져가는 중");
            return false;
        }
        return true;
    }
}
