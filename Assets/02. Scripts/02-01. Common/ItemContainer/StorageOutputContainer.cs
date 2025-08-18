using UnityEngine;

public class StorageOutputContainer : IOutputContainer<Storage>
{
    private GameObject _output;

    public GameObject ServerTakeItem(Storage storage)
    {
        int price = DataTable.Instance.GetIngredientData(storage.IngredientTID).Price;
        if (CurrencyManager.Instance.TrySubtractCurrency(price))
        {
            AudioNetworkManager.Instance.CmdPlaySFX(EStorageAudioType.Buy);
            return CraftItemManager.Instance.TryCreateIngredientItem
                (storage.IngredientTID, storage.transform.position);
        }
        else
        {
            return null;
        }
    }

    public bool ServerCanTake(Storage storage)
    {
        return true;
    }
}
