using UnityEngine;

public interface IBuildingState
{
    public void StartState();
    public void EndState();
    public void ReceivePlaceResult(bool success, uint structureNetId);
    public bool TryAction(Vector3Int gridPosition);
    public void UpdateState(Vector3Int gridPosition);
}