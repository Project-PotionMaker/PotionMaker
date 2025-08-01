using Mirror;
using UnityEngine;

public interface IInteractable<TStructure>
{
    public bool ServerCanInteract(TStructure instance);
    public bool ServerTryInteract(TStructure instance);
}
