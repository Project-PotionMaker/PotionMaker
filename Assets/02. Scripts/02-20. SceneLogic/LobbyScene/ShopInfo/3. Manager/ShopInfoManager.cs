using UnityEngine;

public class ShopInfoManager : MonoBehaviourSingleton<ShopInfoManager>
{
    private ShopInfo _shopInfo;
    public ShopInfo ShopInfo
    {
        get => _shopInfo;
        set => _shopInfo = value;
    }

    private ShopInfoRepository _shopInfoRepository;

    protected override void Awake()
    {
        base.Awake();
    }

    private void InitShopInfoManager()
    {
        _shopInfoRepository = new ShopInfoRepository();
    }


}
