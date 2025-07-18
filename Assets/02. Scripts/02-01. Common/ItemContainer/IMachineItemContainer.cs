using UnityEngine;

public interface IMachineItemContainer
{
    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType);
    public GameObject RequestTakeOutput(Machine machine, MachineStat stat);
}
