using UnityEngine;

public class PickUpTableInteract : IInteractable<Furniture, FurnitureStat>
{
    public bool CanInteract(Furniture instance, FurnitureStat stat)
    {
        return true;
    }

    public bool TryInteract(Furniture instance, FurnitureStat stat)
    {
        if (CanInteract(instance, stat))
        {
            // 여기 놓으면 NPC매니저에서 NPC들 찾아오게
        }
        return true;
    }
}
