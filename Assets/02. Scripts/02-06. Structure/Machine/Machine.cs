using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour, IMachineItemContainer
{
    private MachineStat _stat;
    private IMachineInteractable _interactComponent;

    public void Init(MachineData data, IMachineInteractable interactableComponent)
    {
        MachineStat machineStat = new MachineStat(data);
        _interactComponent = interactableComponent;

        _stat.ClearMachine();
    }

    public bool TryInteract()
    {
        return _interactComponent.TryInteract(this, _stat);
    }

    public bool TryInput(int tid, EInputType inputType)
    {
        if (_stat.InputTIDList.Count + 1 > _stat.Data.MaxInputCount ||
            _stat.IsProcessFinished ||
            _stat.InputTIDList.Contains(tid))
        {
            return false;
        }

        _stat.InputTIDList.Add(tid);

        return true;
    }

    public GameObject TakeOutput()
    {
        if (_stat.IsProcessFinished)
        {
            //여기 Machine에 합쳐버리면 시트 테이블 타입 주는 곳에서 어떻게 판별할까?
            GameObject output = OutputManager.Instance.CreateOutput(_stat.InputTIDList, EInputType.Output);
            _stat.LeftOutputAmount--;
            if (_stat.LeftOutputAmount <= 0)
            {
                _stat.ClearMachine();
            }

            return output;
        }

        return null;
    }
}
