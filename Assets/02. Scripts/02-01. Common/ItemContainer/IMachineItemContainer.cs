using UnityEngine;

public interface IMachineItemContainer
{
    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType);
    public bool CanTakeOut(Machine machine, MachineStat stat);
    public GameObject TakeOutput(Machine machine, MachineStat stat);
}
