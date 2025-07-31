using Mirror;
using System.Collections;
using UnityEngine;

public class AutoProgressInteract : IInteractable<Machine>
{
    public bool CanInteract(Machine machine)
    {
        return (machine.InputTIDList.Count == machine.Data.MaxInputCount && !machine.IsProcessFinished);
    }

    [Command]
    public bool CmdTryInteract(Machine machine)
    {
        return TryInteract(machine);
    }

    public bool TryInteract(Machine machine)
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            machine.transform.Rotate(0, 90, 0);
            return true;
        }
        else
        {
            if (CanInteract(machine) == false)
            {
                return false;
            }

            if (machine.IsProcessStarted)
            {
                machine.IsProcessStarted = false;
                machine.StopAllCoroutines();
            }
            else
            {
                machine.IsProcessStarted = true;
                machine.StartCoroutine(Interact_Coroutine(machine));
            }

            return true;
        }
    }

    public IEnumerator Interact_Coroutine(Machine machine)
    {
        while (machine.CurrentProgress < machine.Data.MaxProgress)
        {
            machine.CurrentProgress += machine.Data.ProgressPerTick * Time.deltaTime;
            //machine.SyncMachineStat();
            yield return null;
        }

    }
}
