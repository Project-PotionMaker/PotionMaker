using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour
{
    [SerializeField]
    private MachineStat _stat;
    private IMachineInteractable _interactComponent;
    private IMachineItemContainer _containerComponent;
    private PhotonView _photonView;

    public void InitMachine(MachineData data, IMachineInteractable interactableComponent, IMachineItemContainer containerComponent)
    {
        _stat = new MachineStat(data);
        _interactComponent = interactableComponent;
        _containerComponent = containerComponent;
        _photonView = GetComponent<PhotonView>();

        _stat.ClearMachine();
        SyncMachineStat(_stat);
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
        return _containerComponent.TakeOutput(this, _stat);
    }

    [ContextMenu("드가자")]
    public void SyncMachineStat(MachineStat stat)
    {
        _photonView.RPC(nameof(RPC_SyncMachineStat), RpcTarget.All, stat);
    }

    [PunRPC]
    public void RPC_SyncMachineStat(MachineStat stat)
    {
        _stat = stat;
    }
}
