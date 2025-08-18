using Mirror;
using UnityEngine;

public interface IGridItemHandler
{
    public void TryInteract(NetworkConnectionToClient conn);

    public void TryPickUp(NetworkConnectionToClient conn);

    public void TryDrop(NetworkConnectionToClient conn, Vector3 targetPosition, NetworkIdentity inputNetId, int tid = 10000, EInputType inputType = EInputType.None);

    public void ResetData();

    public int GetStructureTID();

    public void SetCollider(bool active);

    public void SetHighlight(bool active);

    public void OnIncorrectAction();
}
