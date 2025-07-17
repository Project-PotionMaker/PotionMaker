using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour
{
    private MachineStat _stat;
    private IMachineInteractable _interactComponent;
    private IMachineItemContainer _containerComponent;

    public void Init(MachineData data, IMachineInteractable interactableComponent, IMachineItemContainer containerComponent)
    {
        MachineStat machineStat = new MachineStat(data);
        _interactComponent = interactableComponent;
        _containerComponent = containerComponent;

        _stat.ClearMachine();
    }

    public bool TryInteract()
    {
        return _interactComponent.TryInteract(this, _stat);
    }

    public bool TryInput(int tid, EInputType inputType)
    {
        return _containerComponent.TryInput(this, _stat, tid, inputType);
    }

    public GameObject TakeOutput()
    {
        return _containerComponent.TakeOutput(this, _stat);
    }
}
