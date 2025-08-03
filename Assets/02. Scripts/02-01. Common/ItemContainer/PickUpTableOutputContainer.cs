using UnityEngine;

public class PickUpTableOutputContainer : IOutputContainer<Furniture>
{
    private GameObject _output;

    public GameObject ServerTakeItem(Furniture furniture)
    {
        //GameObject output = stat.InputObject;
        //stat.InputObject = null;
        //CustomerManager.Instance.CommandRemoveOnTable(furniture.netId);
        //return output;
        return null;
    }

    public bool ServerCanTake(Furniture furniture)
    {
        //if(stat.InputObject == null)
        //{
        //    return false;
        //}
        return true;
    }
}
