using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture, FurnitureStat>
{
    public bool TryInput(Furniture furniture, FurnitureStat stat, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if(stat.InputObject == null)
        {
            stat.InputObject = inputObject;
            stat.InputObject.transform.position = stat.InputPosition.position;

            // NPC매니저에서 여기 있는거 알리기

            return true;
        }
        return false;
    }
}
