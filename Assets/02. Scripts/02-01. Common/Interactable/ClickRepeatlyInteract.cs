using System.Collections;
using UnityEngine;

public class ClickRepeatlyInteract : IInteractable<Machine>
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
            machine.ServerSetIsProcessStarted(true);
        }
        IncreaseProgress(machine);
        return true;
    }

    private void IncreaseProgress(Machine machine)
    {
        machine.ServerIncreaseProgress(machine.Data.ProgressPerTick);
    }
}
