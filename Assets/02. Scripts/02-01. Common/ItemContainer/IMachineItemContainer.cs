using UnityEngine;

public interface IMachineItemContainer
{
    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType);
    public bool CanTake(Machine machine, MachineStat stat);
    public GameObject TakeItem(Machine machine, MachineStat stat);
}
