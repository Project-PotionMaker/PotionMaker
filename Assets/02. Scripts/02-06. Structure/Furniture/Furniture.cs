using Photon.Pun;
using System;
using UnityEngine;

public class Furniture : MonoBehaviour, IGridItemHandler
{
    [SerializeField]
    FurnitureStat _stat;
    [SerializeField]
    private Transform _model;

    private IInteractable<Furniture, FurnitureStat> _interactComponent;
    private IInputContainer<Furniture, FurnitureStat> _inputComponent;
    private IOutputContainer<Furniture, FurnitureStat> _outputComponent;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    public Action OnDataChanged;

    public void InitFurniture(FurnitureData data, IInteractable<Furniture, FurnitureStat> interactComponent, IInputContainer<Furniture, FurnitureStat> inputComponent, IOutputContainer<Furniture, FurnitureStat> outputComponent)
    {
        _stat = new FurnitureStat(data);
        _interactComponent = interactComponent;
        _inputComponent = inputComponent;
        _outputComponent = outputComponent;
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
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            if (TryInput(tid, inputType, inputObject))
            {
                return true;
            }
        }
        return false;
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
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            return _interactComponent.TryInteract(this, _stat);
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

    public bool TryInput(int tid, EInputType inputType, GameObject inputObject)
    {
        return _inputComponent.TryInput(this, _stat, tid, inputType, inputObject);
    }
}
