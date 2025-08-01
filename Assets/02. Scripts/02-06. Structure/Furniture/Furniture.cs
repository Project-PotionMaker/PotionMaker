using Mirror;
//using Photon.Pun;
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

    private IInteractable<Furniture> _interactComponent;
    private IInputContainer<Furniture> _inputComponent;
    private IOutputContainer<Furniture> _outputComponent;
    private ICustomerEffectable<Furniture> _effectComponent;

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
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ResetMachineServer;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ResetMachineServer;
    }

    [ClientRpc]
    public void RpcInitFurnitureOnClients(int furnitureTID)
    {
        FurnitureData furnitureData = DataTable.Instance.GetFurnitureData(furnitureTID);
        IInteractable<Furniture> interactable = null;
        IInputContainer<Furniture> inputContainer = null;
        IOutputContainer<Furniture> outputContainer = null;
        ICustomerEffectable<Furniture> customerEffectable = null;
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

    private void InitFurnitureInternal(FurnitureData data, IInteractable<Furniture> interactComponent, IInputContainer<Furniture> inputComponent, IOutputContainer<Furniture> outputComponent, ICustomerEffectable<Furniture> effectComponent)
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

    [Command(requiresAuthority = false)]
    public void CmdTryDrop(Vector3 targetPosition, int tid, EInputType inputType, GameObject inputObject)
    {
        //if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        //{
        //    if (GridManager.Instance.TryPlaceStructure(targetPosition))
        //    {
        //        return true;
        //    }
        //}
        //else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase
        //    ||PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PracticingPhase)
        //{
        //    if (CmdTryInput(tid, inputType, inputObject))
        //    {
        //        return true;
        //    }
        //}
        //return false;
    }

    [Command(requiresAuthority = false)]
    public void CmdTryInteract()
    {
        //if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        //{
        //    _stat.CurrentRotation += 90f;
        //    if (_stat.CurrentRotation >= 360f)
        //    {
        //        _stat.CurrentRotation = 0;
        //    }

        //    _model.rotation = Quaternion.Euler(0, _stat.CurrentRotation, 0);
        //    return true;
        //}
        //else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase
        //    || PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PracticingPhase)
        //{
        //    if(ReferenceEquals(_interactComponent, null) == false)
        //    {
        //        return _interactComponent.ServerTryInteract(this);
        //    }
        //}

        //return false;
    }

    [Command(requiresAuthority = false)]
    public void CmdTryPickUp()
    {
        //if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        //{
        //    return GridManager.Instance.StartPlacement(transform.position);
        //}
        //else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase 
        //    || PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PracticingPhase)
        //{
        //    if (ReferenceEquals(_outputComponent, null) == false)
        //    {
        //        if (_outputComponent.ServerCanTake(this))
        //        {
        //            return _outputComponent.ServerTakeItem(this);
        //        }
        //    }
        //}
        //return null;
    }

    [Command(requiresAuthority = false)]
    public void CmdTryInput(int tid, EInputType inputType, GameObject inputObject)
    {
        //if(ReferenceEquals(_inputComponent, null) == false)
        //{
        //    return _inputComponent.ServerTryInput(this, tid, inputType, inputObject);
        //}
        //return false;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdTryEffect(NetworkIdentity customerIdentity)
    {
        if (ReferenceEquals(_effectComponent, null) == false)
        {
            _effectComponent.ServerEffect(this, customerIdentity);
        }
    }

    public void ResetMachineServer()
    {
        if (!ReferenceEquals(_stat.InputObject, null))
        {
            CraftItemFactory.Instance.CmdReturn(_stat.InputObject);
            _stat.InputObject = null;
        }
    }

    public bool TryInteract()
    {
        //return CmdTryInteract();
        return false;
    }

    public GameObject TryPickUp()
    {
        //return CmdTryPickUp();
        return null;
    }

    public bool TryDrop(Vector3 targetPosition, int tid = 10000, EInputType inputType = EInputType.None, GameObject inputObject = null)
    {
        //return CmdTryDrop(targetPosition, tid, inputType, inputObject);
        return true;
    }

    public void TryEffect(NetworkIdentity customerIdentity)
    {
        CmdTryEffect(customerIdentity);
    }
}
