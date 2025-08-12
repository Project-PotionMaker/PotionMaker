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

    protected override void Awake()
    {
        base.Awake();
    }

    private void FindAllShopInfoSaveables()
    {
        var shopInfoSaveableEnumerator =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IShopInfoSaveable>();

        foreach (var shopInfoSaveable in shopInfoSaveableEnumerator)
        {
            _shopInfoSaveableList.Add(shopInfoSaveable);
        }
    }

    // LobbyScene에서 MakeRoom(방 생성) 버튼을 눌렀을 때 호출해줘야 한다.
    public void InitShopInfoManager(ShopInfo shopInfo)
    {
        if (!ReferenceEquals(shopInfo, null))
        {
            _shopInfo = shopInfo;
        }
        _shopInfoRepository = new ShopInfoRepository();
    }

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
