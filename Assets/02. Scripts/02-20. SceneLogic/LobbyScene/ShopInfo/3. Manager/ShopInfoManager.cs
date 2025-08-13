using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopInfoManager : NetworkBehaviourSingleton<ShopInfoManager>
{
    private ShopInfo _shopInfo;
    public ShopInfo ShopInfo
    {
        get => _shopInfo;
        set => _shopInfo = value;
    }

    private ShopInfoRepository _shopInfoRepository;

    private List<IShopInfoSaveable> _shopInfoSaveableList = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        _shopInfoRepository = new ShopInfoRepository();
        _shopInfo = MirrorNetworkManager.Instance.ShopInfo;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // ShopInfo와 연동되는 매니저가 존재하는 시점에 호출해줘야 한다.
    // 즉, PhaseManager, PotionHouse, CurrencyManager, ReputationManager,
    // SalesManager, GridManger가 존재할 때 호출해줘야 한다.
    // 로딩씬?
    private void FindAllShopInfoSaveables()
    {
        var shopInfoSaveableEnumerator =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IShopInfoSaveable>();

        foreach (var shopInfoSaveable in shopInfoSaveableEnumerator)
        {
            _shopInfoSaveableList.Add(shopInfoSaveable);
        }
    }

    // FindAllShopInfoSaveables()가 성공적으로 작동했다면, 그 다음에 즉시
    // 호출해줘도 무방하다.
    public void ApplyShopInfo()
    {
        foreach (var shopInfoSaveable in _shopInfoSaveableList)
        {
            shopInfoSaveable.ApplyLoadedData(_shopInfo);
        }
    }

    public void SaveShopInfo()
    {
        foreach (var shopInfoSaveable in _shopInfoSaveableList)
        {
            shopInfoSaveable.ProvideSaveData(_shopInfo);
        }
        _shopInfoRepository.Save(_shopInfo);
    }


}
