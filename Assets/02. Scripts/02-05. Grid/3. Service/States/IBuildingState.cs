using UnityEngine;

public interface IBuildingState
{
    public void EndState();
    public bool TryAction(Vector3Int gridPosition);
    public void UpdateState(Vector3Int gridPosition);
}