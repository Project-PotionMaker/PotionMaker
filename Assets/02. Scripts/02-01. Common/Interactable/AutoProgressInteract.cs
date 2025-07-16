using System.Collections;
using UnityEngine;

public class AutoProgressInteract : IMachineInteractable
{
    public virtual bool CanInteract(Machine machine)
    {
        if (machine.InputTIDList.Count == machine.Data.MaxInputCount && machine.IsProcessFinished == false)
        {
            return true;
        }
        return false;
    }


    public bool TryInteract(Machine machine)
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

    public IEnumerator Interact_Coroutine(Machine machine)
    {
        while (machine.CurrentProgress <= machine.Data.MaxProgress)
        {
            machine.CurrentProgress += machine.Data.ProgressPerTick * Time.deltaTime;
            yield return null;
        }

        machine.IsProcessFinished = true;
    }
}
