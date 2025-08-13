using UnityEngine;

public class ClickOnceInteract : IInteractable<Machine>
{
    public bool ServerCanInteract(Machine machine)
    {
        return (machine.InputTIDList.Count == machine.Data.MaxInputCount && !machine.IsProcessFinished);
    }

    public bool ServerTryInteract(Machine machine)
    {
        if (!ServerCanInteract(machine))
        {
            return false;
        }

        if (!machine.IsProcessStarted)
        {
            CompleteProgress(machine);
        }
        return true;
    }

    private void CompleteProgress(Machine machine)
    {
        machine.ServerIncreaseProgress(machine.Data.MaxProgress);

    }
}
