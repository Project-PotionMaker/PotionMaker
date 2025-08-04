using UnityEngine;

public class CasherInteract : IInteractable<Furniture>
{
    private GameObject UI_Market;

    public CasherInteract()
    {
        UI_Market = GameObject.FindAnyObjectByType<UI_Market>().gameObject;
    }

    public bool ServerCanInteract(Furniture instance)
    {
        return true;
    }

    public bool ServerTryInteract(Furniture instance)
    {
        if (ServerCanInteract(instance))
        {
            // NPC매니저에서 기다리는 NPC들 상호작용
            CustomerManager.Instance.CmdRegisterOrder();
        }
        return true;
    }
}
