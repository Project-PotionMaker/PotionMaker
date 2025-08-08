using UnityEngine;

public class PickUpTableOutputContainer : IOutputContainer<Furniture>
{
    public GameObject ServerTakeItem(Furniture furniture)
    {
        GameObject output = furniture.InputObject;
        furniture.InputObject = null;
        if (GridManager.Instance.PickupTableForCustomerList.Contains(furniture.netIdentity))
        {
            CustomerManager.Instance.CmdRemoveOnTable(furniture.netId);
        }
        return output;
    }

    public bool ServerCanTake(Furniture furniture)
    {
        if(furniture.InputObject != null)
        {
            if (GridManager.Instance.PickupTableForCustomerList.Contains(furniture.netIdentity))
            {
                if (CustomerManager.Instance.OrderHandler.PickupTableDict.TryGetValue(furniture.netId, out var pickupTableInfo) && pickupTableInfo.UsingCustomer != null)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}
