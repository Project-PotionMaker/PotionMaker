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

            // NPC매니저에서 여기 있는거 알리기
            CustomerManager.Instance.PlaceOnTable(tid, furniture.PhotonView.ViewID);
            CustomerManager.Instance.ServePotion(tid,furniture.PhotonView.ViewID);
            return true;
        }
        return false;
    }
}
