using Mirror;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Furniture : NetworkBehaviour, IGridItemHandler
{
    [SerializeField]
    private FurnitureStat _stat;
    [SerializeField]
    private Transform _model;

    private IInteractable<Furniture, FurnitureStat> _interactComponent;
    private IInputContainer<Furniture, FurnitureStat> _inputComponent;
    private IOutputContainer<Furniture, FurnitureStat> _outputComponent;
    private ICustomerEffectable<Furniture, FurnitureStat> _effectComponent;

    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    public Action OnDataChanged;

    private void Awake()
    {
        _modelObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            _modelObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
    }

    private void Start()
    {
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ResetItem;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ResetItem;
    }

    [ClientRpc]
    public void RpcInitFurnitureOnClients(int furnitureTID)
    {
        FurnitureData furnitureData = DataTable.Instance.GetFurnitureData(furnitureTID);
        IInteractable<Furniture, FurnitureStat> interactable = null;
        IInputContainer<Furniture, FurnitureStat> inputContainer = null;
        IOutputContainer<Furniture, FurnitureStat> outputContainer = null;
        ICustomerEffectable<Furniture, FurnitureStat> customerEffectable = null;
        // 테스트용
        if (furnitureData.Name == "계산기")
        {
            interactable = new CasherInteract();
        }
        if (furnitureData.Name == "픽업 테이블")
        {
            inputContainer = new PickUpTableInputContainer();
            outputContainer = new PickUpTableOutputContainer();
        }
        if (furnitureData.Name == "허름한 의자" || furnitureData.Name == "푹신한 의자")
        {
            customerEffectable = new ChairEffect();
        }

        InitFurnitureInternal(furnitureData, _interactComponent, _inputComponent, _outputComponent, _effectComponent);
    }

    private void InitFurnitureInternal(FurnitureData data, IInteractable<Furniture, FurnitureStat> interactComponent, IInputContainer<Furniture, FurnitureStat> inputComponent, IOutputContainer<Furniture, FurnitureStat> outputComponent, ICustomerEffectable<Furniture, FurnitureStat> effectComponent)
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
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase
            ||PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PracticingPhase)
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
            if (_stat.CurrentRotation >= 360f)
            {
                _stat.CurrentRotation = 0;
            }

            _model.rotation = Quaternion.Euler(0, _stat.CurrentRotation, 0);
            return true;
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase
            || PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PracticingPhase)
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
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase 
            || PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PracticingPhase)
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
    public void ResetItem()
    {
        if (!ReferenceEquals(_stat.InputObject, null))
        {
            CraftItemFactory.Instance.CmdReturn(_stat.InputObject);
            _stat.InputObject = null;
        }
    }
}
