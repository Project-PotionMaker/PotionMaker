using Mirror;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class AutoProgressInteract : IInteractable<Machine>
{
    public bool ServerCanInteract(Machine machine)
    {
        return (machine.InputTIDList.Count == machine.Data.MaxInputCount && !machine.IsProcessFinished);
    }

    public bool ServerTryInteract(Machine machine)
    {
        if (ServerCanInteract(machine) == false)
        {
            return false;
        }

        if (machine.IsProcessStarted)
        {
            machine.ServerSetIsProcessStarted(false);
            machine.StopAllCoroutines();
            machine.RpcPlayAnimation(EMachineAnimationType.Stop);
        }
        else
        {
            machine.ServerSetIsProcessStarted(true);
            machine.RpcPlayAnimation(EMachineAnimationType.Start);
            machine.StartCoroutine(Interact_CoroutineServer(machine));
        }

        return true;
    }

    public IEnumerator Interact_CoroutineServer(Machine machine)
    {
        while (machine.CurrentProgress < machine.Data.MaxProgress)
        {
            machine.ServerIncreaseProgress(machine.Data.ProgressPerTick * Time.deltaTime);
            yield return null;
        }
    }
}
