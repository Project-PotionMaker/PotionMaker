using UnityEngine;

public interface IInputContainer<TStucture>
{
    public bool ServerTryInput(TStucture instance, int tid, EInputType inputType, GameObject inputObject = null);
}
