using Photon.Pun;
using UnityEngine;

public class DefaultMachineContainer : IMachineItemContainer
{
    protected PhotonView _photonView;
    private GameObject _output;

    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType)
    {
        if (stat.InputTIDList.Count + 1 > stat.Data.MaxInputCount ||
            stat.IsProcessFinished ||
            stat.InputTIDList.Contains(tid))
        {
            return false;
        }

        stat.InputTIDList.Add(tid);

        machine.SyncMachineStat();
        return true;
    }

    public GameObject TakeItem(Machine machine, MachineStat stat)
    {
        if (stat.IsProcessFinished)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                _photonView?.RPC(nameof(RPC_TakeOutput), RpcTarget.MasterClient,
                stat.InputTIDList.ToArray(), stat.Data.TID, EInputType.Output, machine.transform.position);
            }
            else
            {
                RPC_TakeOutput(stat.InputTIDList.ToArray(), stat.Data.TID, EInputType.Output, machine.transform.position);
            }

                stat.LeftOutputAmount--;
            if (stat.LeftOutputAmount <= 0)
            {
                stat.ClearMachine();
            }
            machine.SyncMachineStat();
            return _output;
        }
        return null;
    }

    [PunRPC]
    public void RPC_TakeOutput(int[] TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        _output = CraftItemManager.Instance.TryCreateOutputItem
            (TIDList, machineTID, type, machinePosition);
    }

    public bool CanTake(Machine machine, MachineStat stat)
    {
        return stat.IsProcessFinished;
    }
}
