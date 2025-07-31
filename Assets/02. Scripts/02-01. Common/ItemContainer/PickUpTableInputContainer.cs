using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture, FurnitureStat>
{
    public bool TryInput(Furniture furniture, FurnitureStat stat, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if(stat.InputObject == null)
        {
            stat.InputObject = inputObject;
            stat.InputObject.transform.position = stat.InputPosition.position;
            CustomerManager.Instance.CommandPlaceOnTable(tid, furniture.netId);
            CustomerManager.Instance.CommandServePotion(tid,furniture.netId);
            return true;
        }
        return false;
    }
}
