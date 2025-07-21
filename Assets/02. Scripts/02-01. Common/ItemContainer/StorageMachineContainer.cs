using Photon.Pun;
using UnityEngine;

public class StorageMachineContainer : IOutputInteractable
{
    private GameObject _output;

    public GameObject TakeItem(Machine machine, MachineStat stat)
    {
        if (stat.IsProcessFinished)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                machine.PhotonView.RPC(nameof(RPC_TakeIngredientItem), RpcTarget.MasterClient,
                    stat.InputTIDList[0], machine.transform.position);
            }
            else
            {
                RPC_TakeIngredientItem(stat.InputTIDList[0], machine.transform.position);
            }
            return _output;
        }
        return null;
    }

    [PunRPC]
    public void RPC_TakeIngredientItem(int TID, Vector3 machinePosition)
    {
        _output = CraftItemManager.Instance.TryCreateIngredientItem
            (TID, machinePosition);
    }

    public bool CanTake(Machine machine, MachineStat stat)
    {
        // 창고에서 해당 재료를 빼올 수 있는지 체크하는 부분을 여기 넣어야한다.
        IngredientData data = DataTable.Instance.GetIngredientData(stat.InputTIDList[0]);
        return CurrencyManager.Instance.TrySubtractCurrency(data.Price);
    }
}
