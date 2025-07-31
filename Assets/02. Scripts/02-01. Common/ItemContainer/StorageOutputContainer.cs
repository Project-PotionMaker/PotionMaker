using Photon.Pun;
using UnityEngine;

public class StorageOutputContainer : IOutputContainer<Storage, StorageStat>
{
    private GameObject _output;

    public GameObject TakeItem(Storage storage, StorageStat stat)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    storage.PhotonView.RPC(nameof(RPC_TakeIngredientItem), RpcTarget.MasterClient,
        //        stat.IngredientTID, storage.transform.position);
        //}
        //else
        //{
        //    RPC_TakeIngredientItem(stat.IngredientTID, storage.transform.position);
        //}
        return _output;
    }

    [PunRPC]
    public void RPC_TakeIngredientItem(int ingredientTID, Vector3 machinePosition)
    {
        _output = CraftItemManager.Instance.TryCreateIngredientItem
            (ingredientTID, machinePosition);
    }

    public bool CanTake(Storage storage, StorageStat stat)
    {
        // 창고에서 해당 재료를 빼올 수 있는지 체크하는 부분을 여기 넣어야한다.
        IngredientData data = DataTable.Instance.GetIngredientData(stat.IngredientTID);
        //return CurrencyManager.Instance.TrySubtractCurrency(data.Price);

        return true;
    }
}
