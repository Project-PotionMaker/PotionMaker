using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture>
{
    public bool ServerTryInput(Furniture furniture, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if (furniture.InputObject == null)
        {
            CustomerManager.Instance.CmdPlaceOnTable(tid, furniture.netId);
            CustomerManager.Instance.CmdServePotion(tid, furniture.netId);
            return true;
        }
        return false;
    }
}
