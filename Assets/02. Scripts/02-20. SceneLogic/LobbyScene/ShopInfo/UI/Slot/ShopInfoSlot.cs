using System;
using UnityEngine;

public enum SlotState 
{ 
    Empty, 
    Filled 
}

public class ShopInfoSlot : MonoBehaviour
{
    public event Action<ShopInfo> OnShopInfoCreated;
    public event Action<ShopInfo> OnShopInfoSelected;
    public event Action OnShopInfoUnSelected;
    public event Action OnShopInfoDeleted;

    public SlotState CurrentState { get; private set; } = SlotState.Empty;
    private ShopInfo _shopInfo;

    private CreatePopup _createPopup;

    public void InitShopInfoSlot(CreatePopup createPopup)
    {
        _createPopup = createPopup;
    }

    public void FillShopInfoSlot(ShopInfo shopInfo)
    {
        _shopInfo = shopInfo;
        CurrentState = SlotState.Filled;
        OnShopInfoCreated?.Invoke(_shopInfo);
        Select();
    }

    public void Select()
    {
        OnShopInfoSelected?.Invoke(_shopInfo);
    }

    public void UnSelect()
    {
        OnShopInfoUnSelected?.Invoke();
    }

    public void Delete()
    {
        _shopInfo = null;
        CurrentState = SlotState.Empty;
        OnShopInfoDeleted?.Invoke();
    }
}

