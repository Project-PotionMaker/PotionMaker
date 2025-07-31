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
    private List<ShopInfoSlot> _shopInfoSlotList = new();

    private ShopInfo _selectedShopInfo;

    private void Start()
    {
        InitShopInfoHandler();
    }

    private void InitShopInfoHandler()
    {
        _createPopup.OnCreateNewShopInfo += UpdateSelectedShopInfo;
        foreach (var slot in _shopInfoSlotList)
        {
            slot.InitShopInfoSlot(_createPopup);
            slot.OnShopInfoSelected += UpdateSelectedShopInfo;
            slot.OnShopInfoDeleted += DeleteSelectedShopInfo;
        }
    }

    public void UpdateSelectedShopInfo(ShopInfoSlot shopInfoSlot, ShopInfo shopInfo, bool isCreated)
    {
        _selectedShopInfo = shopInfo;
        foreach (var slot in _shopInfoSlotList)
        {
            if (ReferenceEquals(slot, shopInfoSlot))
            {
                if (isCreated)
                {
                    slot.CreateShopInfo(shopInfo);
                }
                else
                {
                }
            }
            else
            {

            }
        }
    }

    public void UpdateSelectedShopInfo()
    {

    }


    private void DeleteSelectedShopInfo()
    {
        _selectedShopInfo = null;
    }
}
