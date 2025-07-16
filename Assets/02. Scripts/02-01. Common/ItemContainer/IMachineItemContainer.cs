using UnityEngine;

public interface IMachineItemContainer
{
    public bool TryInput(int tid, EInputType inputType);
    public GameObject TakeOutput();
}
