using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture, FurnitureStat>
{
    public bool TryInput(Furniture furniture, FurnitureStat stat, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if(inputType != EInputType.Potion)
        {
            return false;
        }
        else if(stat.InputObject == null)
        {
            stat.InputObject = inputObject;
            stat.InputObject.transform.position = stat.InputPosition.position;

            CustomerManager.Instance.ServePotion(tid,furniture.PhotonView.ViewID);
            return true;
        }
        return false;
    }
}
