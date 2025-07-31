using Photon.Pun;
using UnityEngine;

public class MachineOutputContainer : IOutputContainer<Machine, MachineStat>
{
    private GameObject _output;

    public GameObject TakeItem(Machine machine, MachineStat stat)
    {
        if (stat.IsProcessFinished)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                //machine.PhotonView.RPC(nameof(RPC_TakeOutput), RpcTarget.MasterClient,
                //stat.InputTIDList.ToArray(), stat.Data.TID, stat.InputType, machine.transform.position);
            }
            else
            {
                RPC_TakeOutput(stat, stat.InputTIDList.ToArray(), stat.Data.TID, stat.InputType, machine.transform.position);
            }

            machine.SyncMachineStat();
            return _output;
        }
        return null;
    }

    [PunRPC]
    public void RPC_TakeOutput(MachineStat stat, int[] TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        if(stat.Data.Name == "병입기")
        {
            _output = CraftItemManager.Instance.TryCreatePotionItem(TIDList, machineTID, machinePosition);
        }
        else
        {
            _output = CraftItemManager.Instance.TryCreateOutputItem(TIDList, machineTID, type, machinePosition);
        }


        stat.LeftOutputAmount--;
        if (stat.LeftOutputAmount <= 0)
        {
            stat.ClearMachine();
        }
    }

    public bool CanTake(Machine machine, MachineStat stat)
    {
        return stat.IsProcessFinished;
    }
}
