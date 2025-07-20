using Google.Apis.Sheets.v4.Data;
using NUnit.Framework;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour, IGridItemHandler
{
    [SerializeField]
    private MachineStat _stat;
    [SerializeField]
    private Transform _model;

    private IInteractable<Machine, MachineStat> _interactComponent;
    private IInputContainer<Machine, MachineStat> _inputComponent;
    private IOutputContainer<Machine, MachineStat> _outputComponent;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    public Action OnDataChanged;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void InitMachine(MachineData data, IInteractable<Machine, MachineStat> interactableComponent, IInputContainer<Machine, MachineStat> inputComponent, IOutputContainer<Machine, MachineStat> outputComponent)
    {
        _stat = new MachineStat(data);
        _interactComponent = interactableComponent;
        _inputComponent = inputComponent;
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
            _model.rotation = Quaternion.Euler(0, _stat.CurrentRotation, 0);
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            return _interactComponent.TryInteract(this, _stat);
        }

        return false;
    }

    private bool TryInput(int tid, EInputType inputType)
    {
        return _inputComponent.TryInput(this, _stat, tid, inputType);
    }

    [ContextMenu("HI")]
    public void SyncMachineStat()
    {
        _photonView.RPC(nameof(RPC_SyncMachineStat), RpcTarget.All, _stat.CurrentProgress, _stat.LeftOutputAmount, _stat.IsProcessFinished, _stat.IsProcessStarted, _stat.InputTIDList.ToArray());
    }

    [PunRPC]
    public void RPC_SyncMachineStat(float currentProgress, int leftOutputAmount, bool isProcessFinished, bool isProcessStarted, int[] inputTIDList)
    {
        _stat.CurrentProgress = currentProgress;
        _stat.LeftOutputAmount = leftOutputAmount;
        _stat.IsProcessFinished = isProcessFinished;
        _stat.IsProcessStarted = isProcessStarted;
        _stat.InputTIDList = inputTIDList.ToList();

        OnDataChanged?.Invoke();
    }

    // UI 테스트용
    public MachineStat GetStat()
    {
        return _stat;
    }

    public GameObject TryPickUp()
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            return GridManager.Instance.StartPlacement(transform.position);
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            if(ReferenceEquals(_outputComponent, null) == false)
            {
                if(_outputComponent.CanTake(this, _stat))
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
        else if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            if(TryInput(tid, inputType))
            {
                return true;
            }
        }
        return false;
    }
}
