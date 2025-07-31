using Photon.Pun;
using UnityEngine;

public class MachineOutputContainer : IOutputContainer<Machine>
{
    private GameObject _output;

    public GameObject TakeItem(Machine machine)
    {
        if (machine.IsProcessFinished)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                //machine.PhotonView.RPC(nameof(RPC_TakeOutput), RpcTarget.MasterClient,
                //stat.InputTIDList.ToArray(), stat.Data.TID, stat.InputType, machine.transform.position);
            }
            else
            {
                RPC_TakeOutput(machine, machine.InputTIDList.ToArray(), machine.Data.TID, machine.InputType, machine.transform.position);
            }

            return _output;
        }
        return null;
    }

    [PunRPC]
    public void RPC_TakeOutput(Machine machine, int[] TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        if(machine.Data.Name == "병입기")
        {
            _output = CraftItemManager.Instance.TryCreatePotionItem(TIDList, machineTID, machinePosition);
        }
        else
        {
            _output = CraftItemManager.Instance.TryCreateOutputItem(TIDList, machineTID, type, machinePosition);
        }


        machine.LeftOutputAmount--;
        if (machine.LeftOutputAmount <= 0)
        {
            machine.ResetMachineServer();
        }
    }

    public bool CanTake(Machine machine)
    {
        return machine.IsProcessFinished;
    }
}
