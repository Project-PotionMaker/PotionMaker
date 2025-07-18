using UnityEngine;

public class ClickOnceInteract : IMachineInteractable
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
            CompleteProgress(machine, stat);
        }
        return true;
    }

    private void CompleteProgress(Machine machine, MachineStat stat)
    {
        stat.CurrentProgress = stat.Data.MaxProgress;
    }
}
