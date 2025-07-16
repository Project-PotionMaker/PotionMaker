using System.Collections;
using UnityEngine;

public class Distiller : Machine
{
    public override bool TryInput(int tid, EInputType inputType)
    {
        if (_isFinished || InputTIDList.Contains(tid))
        {
            return false;
        }

        InputTIDList.Add(tid);
        return true;
    }

    public override bool TryInteract()
    {
        if(CanInteract() == false)
        {
            return false;
        }

        if (_isStarted)
        {
            _isStarted = false;
            StopAllCoroutines();
        }
        else
        {
            _isStarted = true;
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

        _isFinished = true;
    }

    public override GameObject TakeOutput()
    {
        if (_isFinished)
        {
            // 아웃풋매니저에서 StructureManger처럼 풀에서 가져와서 새로운 아웃풋 생성

            _leftOutputAmount--;
            if(_leftOutputAmount <= 0)
            {
                ClearMachine();
            }
            // return ~~;
        }

        return null;
    }


}
