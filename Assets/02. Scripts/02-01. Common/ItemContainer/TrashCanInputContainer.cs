using UnityEngine;

public class TrashCanInputContainer : IInputContainer<Furniture>
{
    public bool ServerTryInput(Furniture instance, int tid, EInputType inputType, GameObject inputObject)
    {
        if(inputObject != null)
        {
            CraftItemFactory.Instance.ReturnObject(inputObject);
        }
        return true;
    }
}
