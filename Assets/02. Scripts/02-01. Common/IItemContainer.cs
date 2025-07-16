using UnityEngine;

public interface IItemContainer
{
    public bool TryInput(int tid, EInputType inputType);
    public GameObject TakeOutput();
}
