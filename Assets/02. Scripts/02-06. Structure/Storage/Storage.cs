using Mirror;
//using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Storage : NetworkBehaviour, IGridItemHandler
{
    private StorageStat _stat;
    [SerializeField]
    private Transform _model;
    private IOutputContainer<Storage> _outputComponent;

    public Action<StorageStat> OnDataChanged;
    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    private void Awake()
    {
        _modelObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            _modelObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
    }

    [ClientRpc]
    public void RpcInitStorageOnClients(int storageTID, int ingredientTID)
    {
        StorageData storageData = DataTable.Instance.GetStorageData(storageTID);
        IOutputContainer<Storage> outputInteractable = new StorageOutputContainer();
        InitStorageInternal(storageData, ingredientTID, outputInteractable);
    }

    private void InitStorageInternal(StorageData data, int ingredientTID, IOutputContainer<Storage> outputComponent)
    {
        _stat = new StorageStat(data, ingredientTID);
        _outputComponent = outputComponent;

        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            if (modelInfo.TID == _stat.Data.TID)
            {
                modelInfo.Model.SetActive(true);
            }
        }

        OnDataChanged?.Invoke(_stat);
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
    public void CmdTryDrop(Vector3 targetPosition, int tid, EInputType inputType, GameObject inputObject)
    {
        //if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        //{
        //    if (GridManager.Instance.TryPlaceStructure(targetPosition))
        //    {
        //        return true;
        //    }
        //}
        //return false;
    }
    public void ResetMachineServer()
    {
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
        return false;
    }
}
