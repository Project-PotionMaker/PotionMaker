using UnityEngine;

public interface IOutputContainer<TClass, TStat>
{
    public bool CanTake(TClass instance, TStat stat);
    public GameObject TakeItem(TClass instance, TStat stat);
}
