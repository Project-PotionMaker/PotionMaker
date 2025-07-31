//using Photon.Pun;
using UnityEngine;

public class PickUpTableOutputContainer : IOutputContainer<Furniture>
{
    private GameObject _output;

    public GameObject ServerTakeItem(Furniture furniture)
    {
        //GameObject output = stat.InputObject;
        //stat.InputObject = null;
        //CustomerManager.Instance.RemoveOnTable(furniture.PhotonView.ViewID);
        //return output;
        return null;
    }

    public bool ServerCanTake(Furniture furniture)
    {
        //if(stat.InputObject == null)
        //{
        //    return false;
        //}
        //if (CustomerManager.Instance.OrderHandler.PickupTableDict[furniture.PhotonView.ViewID].UsingCustomer != null)
        //{
        //    Debug.Log("이미 가져가는 중");
        //    return false;
        //}
        return true;
    }
}
