using UnityEngine;

public class PickUpTableInteract : IInteractable<Furniture>
{
    public bool ServerCanInteract(Furniture instance)
    {
        return true;
    }

    public bool ServerTryInteract(Furniture instance)
    {
        if (ServerCanInteract(instance))
        {
            // 여기 놓으면 NPC매니저에서 NPC들 찾아오게
        }
        return true;
    }
}
