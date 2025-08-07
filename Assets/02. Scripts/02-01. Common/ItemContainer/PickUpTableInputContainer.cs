using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture>
{
    public bool ServerTryInput(Furniture furniture, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if (furniture.InputObject == null)
        {
            if (GridManager.Instance.PickupTableForCustomerList.Contains(furniture.netIdentity))
            {
                CustomerManager.Instance.CmdPlaceOnTable(tid, furniture.netId);
                CustomerManager.Instance.CmdServePotion(tid, furniture.netId);
            }
            return true;
        }
        return false;
    }
}
