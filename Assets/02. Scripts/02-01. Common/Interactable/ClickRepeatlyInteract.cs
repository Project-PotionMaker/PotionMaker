using System.Collections;
using UnityEngine;

public class ClickRepeatlyInteract : IInteractable<Machine>
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
            machine.IsProcessStarted = true;
        }
        IncreaseProgress(machine);
        return true;
    }

    private void IncreaseProgress(Machine machine)
    {
        machine.CurrentProgress += machine.Data.ProgressPerTick;
    }
}
