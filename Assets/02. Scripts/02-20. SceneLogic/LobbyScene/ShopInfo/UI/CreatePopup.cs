using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class CreatePopup : MonoBehaviour
{
    [Header("Project")]
    [SerializeField]
    private TMP_InputField _inputFieldShopName;

    private int _selectedSlotIndex;

    private ShopInfoSlot _selectedShopInfoSlot;

    public event Action<ShopInfoSlot, ShopInfo> OnShopInfoCreated;

    public void OpenPopup(ShopInfoSlot shopInfoSlot)
    {
        gameObject.SetActive(true);
        _inputFieldShopName.text = string.Empty;
        _selectedShopInfoSlot = shopInfoSlot;
        _selectedSlotIndex = shopInfoSlot.SlotIndex;
    }

    public void CreateNewShopInfo()
    {
        if (string.IsNullOrEmpty(_inputFieldShopName.text))
        {
            Debug.LogWarning("방 제목은 빈 문자열일 수 없습니다.");
            return;
        }
        ShopInfo newShopInfo = new ShopInfo(_inputFieldShopName.text, _selectedSlotIndex);
        OnShopInfoCreated?.Invoke(_selectedShopInfoSlot, newShopInfo);
        ClosePopup();
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
