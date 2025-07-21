using UnityEngine;

public interface IInteractable<TClass, TStat>
{
    public bool CanInteract(TClass instance, TStat stat);
    public bool TryInteract(TClass instance, TStat stat);
}
