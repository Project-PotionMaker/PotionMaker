using System.Collections;
using UnityEngine;

public class AutoProgressInteract : IMachineInteractable
{
    public virtual bool CanInteract(Machine machine)
    {
        if (machine.InputTIDList.Count == _data.MaxInputCount && _isProcessFinished == false)
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

        if (_isProcessStarted)
        {
            _isProcessStarted = false;
            StopAllCoroutines();
        }
        else
        {
            _isProcessStarted = true;
           machine.StartCoroutine(Interact_Coroutine(machine));
        }
        return true;
    }

    public IEnumerator Interact_Coroutine(Machine machine)
    {
        while (_currentProgress <= Data.MaxProgress)
        {
            _currentProgress += Data.ProgressPerTick * Time.deltaTime;
            yield return null;
        }

        _isProcessFinished = true;
    }
}
