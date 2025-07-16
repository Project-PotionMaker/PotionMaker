using UnityEngine;

public interface IMachineItemContainer
{
    public bool TryInput(Machine machine, int tid, EInputType inputType);
    public GameObject TakeOutput(Machine machine);
}
