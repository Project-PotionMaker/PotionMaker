using UnityEngine;

public class PickUpTableOutputContainer : IOutputContainer<Furniture>
{
    private GameObject _output;

    public GameObject ServerTakeItem(Furniture furniture)
    {
        GameObject output = furniture.InputObject;
        furniture.InputObject = null;
        CustomerManager.Instance.CmdRemoveOnTable(furniture.netId);
        return output;
    }

    public bool ServerCanTake(Furniture furniture)
    {
        if (CustomerManager.Instance.OrderHandler.PickupTableDict[furniture.netId].UsingCustomer != null)
        {
            Debug.LogWarning("Cannot take item from pickup table, customer is using it.");
            return false;
        }
        return true;
    }
}
