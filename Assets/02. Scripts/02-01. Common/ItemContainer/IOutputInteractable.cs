using UnityEngine;

public interface IOutputInteractable
{
    public bool CanTake(Machine machine, MachineStat stat);
    public GameObject TakeItem(Machine machine, MachineStat stat);
}
