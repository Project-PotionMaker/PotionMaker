using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Furniture : MonoBehaviour, IGridItemHandler
{
    [SerializeField]
    private FurnitureStat _stat;
    [SerializeField]
    private Transform _model;

    private IInteractable<Furniture, FurnitureStat> _interactComponent;
    private IInputContainer<Furniture, FurnitureStat> _inputComponent;
    private IOutputContainer<Furniture, FurnitureStat> _outputComponent;
    private ICustomerEffectable<Furniture, FurnitureStat> _effectComponent;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    public Action OnDataChanged;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _modelObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            _modelObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
    }

    public void InitFurniture(FurnitureData data, IInteractable<Furniture, FurnitureStat> interactComponent, IInputContainer<Furniture, FurnitureStat> inputComponent, IOutputContainer<Furniture, FurnitureStat> outputComponent, ICustomerEffectable<Furniture, FurnitureStat>effectComponent)
    {
        _stat = new FurnitureStat(data, _stat.InputPosition);
        _interactComponent = interactComponent;
        _inputComponent = inputComponent;
        _outputComponent = outputComponent;
        _effectComponent = effectComponent;

        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            if (modelInfo.TID == _stat.Data.TID)
            {
                _model = modelInfo.Model.transform;
                modelInfo.Model.SetActive(true);
            }
        }
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

            _model.rotation = Quaternion.Euler(0, _stat.CurrentRotation, 0);
            return true;
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            if(ReferenceEquals(_interactComponent, null) == false)
            {
                return _interactComponent.TryInteract(this, _stat);
            }
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
        if(ReferenceEquals(_inputComponent, null) == false)
        {
            return _inputComponent.TryInput(this, _stat, tid, inputType, inputObject);
        }
        return false;
    }

    public void TryEffect(Customer customer)
    {
        if (ReferenceEquals(_effectComponent, null) == false)
        {
            _effectComponent.Effect(this, _stat, customer);
        }
    }
}
