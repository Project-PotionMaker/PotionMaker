using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture>
{
    public bool ServerTryInput(Furniture furniture, int tid, EInputType inputType, GameObject inputObject = null)
    {
        //if(stat.InputObject == null)
        //{
        //    stat.InputObject = inputObject;
        //    stat.InputObject.transform.position = stat.InputPosition.position;

        //    //CustomerManager.Instance.ServePotion(tid,furniture.PhotonView.ViewID);
        //    return true;
        //}
        return false;
    }
}
