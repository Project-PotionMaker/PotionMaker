using UnityEngine;

public interface IInteractable
{
    public bool CanInteract();
    public bool TryInteract();
}
