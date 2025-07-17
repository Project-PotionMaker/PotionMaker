using UnityEngine;

public interface IMachineInteractable
{
    public bool CanInteract(Machine machine, MachineStat stat);
    public bool TryInteract(Machine machine, MachineStat stat);
}
