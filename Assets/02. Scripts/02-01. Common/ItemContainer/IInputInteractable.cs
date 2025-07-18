using UnityEngine;

public interface IInputInteractable
{
    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType);
}
