using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopInfoHandler : MonoBehaviour
{
    public event Action<ShopInfo> OnShopInfoUpdated;

    [Header("Hierarchy")]
    [SerializeField]
    private CreatePopup _createPopup;


    [Header("Project")]
    [SerializeField]
    private List<ShopInfoSlot> _shopInfoList = new();

    private ShopInfo _selectedShopInfo;
    public ShopInfo SelectedShopInfo => _selectedShopInfo;

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
            slot.OnShopInfoSelected += ChangeShopInfo;
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
        OnShopInfoUpdated?.Invoke(shopInfo);
    }

    public void ChangeShopInfo(ShopInfo shopInfo)
    {
        _selectedShopInfo = shopInfo;
        OnShopInfoUpdated?.Invoke(shopInfo);
    }

    private void DeleteSelectedShopInfo()
    {
        _selectedShopInfo = null;
        OnShopInfoUpdated?.Invoke(null);
    }
}
