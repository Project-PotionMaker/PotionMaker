using System.Collections;
using UnityEngine;

public class ClickRepeatlyInteract : IMachineInteractable
{
    public bool CanInteract(Machine machine, MachineStat stat)
    {
        return (stat.InputTIDList.Count == stat.Data.MaxInputCount && !stat.IsProcessFinished);
    }

    public bool TryInteract(Machine machine, MachineStat stat)
    {
        if (!CanInteract(machine, stat))
        {
            return false;
        }

        if (!stat.IsProcessStarted)
        {
            stat.IsProcessStarted = true;
        }
        IncreaseProgress(machine, stat);
        return true;
    }

    private void IncreaseProgress(Machine machine, MachineStat stat)
    {
        stat.CurrentProgress += stat.Data.ProgressPerTick;
    }
}
