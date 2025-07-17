using UnityEngine;

public interface IGridPlaceable
{
    public bool TryPickUp();
    public bool TryDrop();
}
