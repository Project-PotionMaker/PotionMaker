using UnityEngine;

public interface IGridItemHandler
{
    public bool TryInteract();

    public GameObject TryPickUp();

    public bool TryDrop(Vector3 targetPosition, int tid = 10000, EInputType inputType = EInputType.None, GameObject inputObject = null);

    public void ResetMachineServer();
}
