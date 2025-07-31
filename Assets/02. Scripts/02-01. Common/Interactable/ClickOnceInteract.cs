using UnityEngine;

public class ClickOnceInteract : IInteractable<Machine>
{
    public bool CanInteract(Machine machine)
    {
        return (machine.InputTIDList.Count == machine.Data.MaxInputCount && !machine.IsProcessFinished);
    }

    public bool TryInteract(Machine machine)
    {
        if (!CanInteract(machine))
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
        machine.CurrentProgress = machine.Data.MaxProgress;
    }
}
