using UnityEngine;

public class CasherInteract : IInteractable<Furniture, FurnitureStat>
{
    public bool CanInteract(Furniture instance, FurnitureStat stat)
    {
        return true;
    }

    public bool TryInteract(Furniture instance, FurnitureStat stat)
    {
        Debug.Log("계산기 상호작용 시도");
        if (CanInteract(instance, stat))
        {
            // NPC매니저에서 기다리는 NPC들 상호작용
            CustomerManager.Instance.CommandRegisterOrder();
        }
        return true;
    }
}
