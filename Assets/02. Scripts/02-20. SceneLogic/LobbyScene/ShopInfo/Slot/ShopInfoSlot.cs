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
    public event Action<ShopInfoSlot, ShopInfo, bool> OnShopInfoSelected;
    public event Action OnShopInfoUnSelected;
    public event Action OnShopInfoDeleted;

    public SlotState CurrentState { get; private set; } = SlotState.Empty;
    private ShopInfo _shopInfo;

    private CreatePopup _createPopup;

    public void InitShopInfoSlot(CreatePopup createPopup)
    {
        _createPopup = createPopup;
    }

    public void CreateShopInfo(ShopInfo shopInfo)
    {
        _shopInfo = shopInfo;
        CurrentState = SlotState.Filled;
        OnShopInfoCreated.Invoke(_shopInfo);
    }
    
    public void OnSlotButtonClick()
    {
        switch (CurrentState)
        {
            case SlotState.Empty:
                _createPopup.OpenPopup(this);
                break;
            case SlotState.Filled:
                OnShopInfoSelected?.Invoke(this, _shopInfo, false);
                break;
        }
    }

    public void OnDeleteButtonClick()
    {
        _shopInfo = null;
        CurrentState = SlotState.Empty;
        OnShopInfoDeleted?.Invoke();
    }
}

