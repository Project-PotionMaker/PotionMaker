using UnityEngine;

public interface IShopInfoSaveable
{
    public void ApplyLoadedData(ShopInfo shopInfo);

    public void ProvideSaveData(ShopInfo shopInfo);
}
