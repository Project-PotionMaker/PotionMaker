using UnityEngine;

public interface IOutputContainer<TStructure>
{
    public bool ServerCanTake(TStructure instance);
    public GameObject ServerTakeItem(TStructure instance);
}
