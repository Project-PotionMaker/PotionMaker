using UnityEngine;

public class StorageOutputContainer : IOutputContainer<Storage>
{
    private GameObject _output;

    public GameObject ServerTakeItem(Storage storage)
    {
        return CraftItemManager.Instance.TryCreateIngredientItem
            (storage.IngredientTID, storage.transform.position);
    }

    public bool ServerCanTake(Storage storage)
    {
        return true;
    }
}
