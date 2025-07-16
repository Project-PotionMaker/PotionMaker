using UnityEngine;

public interface IMachineInteractable
{
    public bool CanInteract(Machine machine);
    public bool TryInteract(Machine machine);
}
