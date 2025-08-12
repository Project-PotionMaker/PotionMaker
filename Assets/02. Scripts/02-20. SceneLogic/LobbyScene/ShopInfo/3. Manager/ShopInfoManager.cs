using UnityEngine;

public class ShopInfoManager : MonoBehaviourSingleton<ShopInfoManager>
{
    private ShopInfo _shopInfo;
    public ShopInfo ShopInfo
    {
        get => _shopInfo;
        set => _shopInfo = value;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    
}
