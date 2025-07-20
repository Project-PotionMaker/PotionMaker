using Photon.Pun;
using System;
using UnityEngine;

public class Storage : MonoBehaviour, IGridItemHandler
{
    private StorageStat _stat;
    private IOutputContainer<Storage, StorageStat> _outputComponent;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    public Action OnDataChanged;

    public void InitStorage(StorageData data, int ingredientTID, IOutputContainer<Storage, StorageStat> outputComponent)
    {
        _stat = new StorageStat(data, ingredientTID);
        _outputComponent = outputComponent;
        _photonView = GetComponent<PhotonView>();
    }

    public bool TryInteract()
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            _stat.CurrentRotation += 90f;
            if (_stat.CurrentRotation > 360f)
            {
                _stat.CurrentRotation = 0;
            }

            transform.rotation = Quaternion.Euler(0, _stat.CurrentRotation, 0);
            return true;
        }
        return false;
    }

    public GameObject TryPickUp()
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            return GridManager.Instance.StartPlacement(transform.position);
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            if (ReferenceEquals(_outputComponent, null) == false)
            {
                if (_outputComponent.CanTake(this, _stat))
                {
                    return _outputComponent.TakeItem(this, _stat);
                }
            }
        }
        return null;
    }

    public bool TryDrop(Vector3 targetPosition, int tid = 10000, EInputType inputType = EInputType.None, GameObject inputObject = null)
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            if (GridManager.Instance.TryPlaceStructure(targetPosition))
            {
                return true;
            }
        }
        return false;
    }
}
