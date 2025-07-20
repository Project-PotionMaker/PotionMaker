using UnityEngine;

public interface IInputContainer<TClass, TStat>
{
    public bool TryInput(TClass instance, TStat stat, int tid, EInputType inputType);
}
