using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class ShopInfoHandler : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private CreatePopup _createPopup;


    [Header("Project")]
    [SerializeField]
    private List<ShopInfoSlot> _shopInfoList = new();

    private ShopInfo _selectedShopInfo;

    private void Start()
    {
        InitShopInfoHandler();
    }

    private void InitShopInfoHandler()
    {
        _createPopup.OnShopInfoCreated += UpdateShopInfoList;
        foreach (var slot in _shopInfoList)
        {
            slot.InitShopInfoSlot(_createPopup);
            slot.OnShopInfoDeleted += DeleteSelectedShopInfo;
        }
    }

    public void UpdateShopInfoList(ShopInfoSlot shopInfoSlot, ShopInfo shopInfo)
    {
        _selectedShopInfo = shopInfo;
        foreach (var slot in _shopInfoList)
        {
            if (ReferenceEquals(slot, shopInfoSlot))
            {
                slot.FillShopInfoSlot(shopInfo);

            }
            else
            {
                slot.UnSelect();
            }
        }
    }

    private void DeleteSelectedShopInfo()
    {
        _selectedShopInfo = null;
    }
}
