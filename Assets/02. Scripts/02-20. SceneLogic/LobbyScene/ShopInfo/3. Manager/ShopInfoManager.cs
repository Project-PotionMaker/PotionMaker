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

    public void InitShopInfoManager(ShopInfo shopInfo)
    {
        if (!ReferenceEquals(shopInfo, null))
        {
            _shopInfo = shopInfo;
        }
        _shopInfoRepository = new ShopInfoRepository();
    }

    public void SaveShopInfo()
    {
        ShopInfoSaveEvents.TriggerSaveRequest(_shopInfo);
        _shopInfoRepository.Save(_shopInfo);
    }


}
