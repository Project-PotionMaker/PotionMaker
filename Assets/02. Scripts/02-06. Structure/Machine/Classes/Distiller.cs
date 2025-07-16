using System.Collections;
using UnityEngine;

public class Distiller : Machine
{
    public override bool TryInput(int tid, EInputType inputType)
    {
        if (_isProcessFinished || InputTIDList.Contains(tid))
        {
            return false;
        }

        InputTIDList.Add(tid);

        ChangeMesh(1);
        return true;
    }

    public override bool TryInteract()
    {
        if(CanInteract() == false)
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
            StartCoroutine(Interact_Coroutine());
        }
        return true;
    }

    public IEnumerator Interact_Coroutine()
    {
        while(_currentProgress <= Data.MaxProgress)
        {
            _currentProgress += Data.ProgressPerTick * Time.deltaTime;
            yield return null;
        }

        ChangeMesh(2);
        _isProcessFinished = true;
    }

    public override GameObject TakeOutput()
    {
        if (_isProcessFinished)
        {
            GameObject output = OutputManager.Instance.CreateOutput(InputTIDList, EInputType.Output);
            _leftOutputAmount--;
            if(_leftOutputAmount <= 0)
            {
                ClearMachine();
            }

            return output;
        }

        return null;
    }


}
