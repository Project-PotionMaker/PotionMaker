using System.Collections;
using UnityEngine;

public class AutoProgressInteract : IMachineInteractable
{
    public bool CanInteract(Machine machine, MachineStat stat)
    {
        return (stat.InputTIDList.Count == stat.Data.MaxInputCount && !stat.IsProcessFinished);
    }

    public bool TryInteract(Machine machine, MachineStat stat)
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            machine.transform.Rotate(0, 90, 0);
            return true;
        }
        else
        {
            if (CanInteract(machine, stat) == false)
            {
                return false;
            }

            if (stat.IsProcessStarted)
            {
                stat.IsProcessStarted = false;
                machine.StopAllCoroutines();
            }
            else
            {
                stat.IsProcessStarted = true;
                machine.StartCoroutine(Interact_Coroutine(machine, stat));
            }

            machine.SyncMachineStat();
            return true;
        }
    }

    public IEnumerator Interact_Coroutine(Machine machine, MachineStat stat)
    {
        while (stat.CurrentProgress <= stat.Data.MaxProgress)
        {
            stat.CurrentProgress += stat.Data.ProgressPerTick * Time.deltaTime;
            machine.SyncMachineStat();
            yield return null;
        }
    }
}
