using NUnit.Framework;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour
{
    [SerializeField]
    private MachineStat _stat;
    private IMachineInteractable _interactComponent;
    private IMachineItemContainer _containerComponent;
    private PhotonView _photonView;

    public Action OnDataChanged;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void InitMachine(MachineData data, IMachineInteractable interactableComponent, IMachineItemContainer containerComponent)
    {
        _stat = new MachineStat(data);
        _interactComponent = interactableComponent;
        _containerComponent = containerComponent;
        _photonView = GetComponent<PhotonView>();
    }

    public bool TryInteract()
    {
        return _interactComponent.TryInteract(this, _stat);
    }

    public bool TryInput(int tid, EInputType inputType)
    {
        return _containerComponent.TryInput(this, _stat, tid, inputType);
    }

    public GameObject TakeOutput()
    {
        return _containerComponent.RequestTakeOutput(this, _stat);
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
}
