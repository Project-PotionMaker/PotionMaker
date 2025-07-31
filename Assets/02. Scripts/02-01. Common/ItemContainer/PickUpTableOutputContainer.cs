using Photon.Pun;
using UnityEngine;

public class PickUpTableOutputContainer : IOutputContainer<Furniture, FurnitureStat>
{
    private GameObject _output;

    public GameObject TakeItem(Furniture furniture, FurnitureStat stat)
    {
        GameObject output = stat.InputObject;
        stat.InputObject = null;
        CustomerManager.Instance.CommandRemoveOnTable(furniture.netId);
        return output;
    }

    public bool CanTake(Furniture furniture, FurnitureStat stat)
    {
        if(stat.InputObject == null)
        {
            return false;
        }
        return true;
    }
}
