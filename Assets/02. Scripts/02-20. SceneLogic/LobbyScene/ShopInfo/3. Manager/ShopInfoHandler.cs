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

    private ShopInfoRepository _shopInfoRepository;

    private void Awake()
    {
        _createPopup.OnShopInfoCreated += UpdateShopInfoList;
    }

    private void Start()
    {
        InitShopInfoHandler();
    }

    private void InitShopInfoHandler()
    {
        _shopInfoRepository = new ShopInfoRepository();
        List<ShopInfo> loadedShopInfoList = _shopInfoRepository.Load();
        int slotIndex = 0;
        foreach (var slot in _shopInfoList)
        {
            slot.OnShopInfoSelected += ChangeShopInfo;
            slot.OnShopInfoDeleted += DeleteSelectedShopInfo;
            slot.InitShopInfoSlot(_createPopup, loadedShopInfoList[slotIndex]);
            slotIndex++;
        }
        UpdateShopInfoList(_shopInfoList[0], loadedShopInfoList[0]);
    }

    public void UpdateShopInfoList(ShopInfoSlot shopInfoSlot, ShopInfo shopInfo)
    {
        _selectedShopInfo = shopInfo;
        foreach (var slot in _shopInfoList)
        {
            if (ReferenceEquals(slot, shopInfoSlot))
            {
                slot.UpdateShopInfoSlot(shopInfo);

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
