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

    private ShopInfoSlot _selectedShopInfoSlot;

    public event Action<ShopInfoSlot, ShopInfo, bool> OnCreateNewShopInfo;

    public void OpenPopup(ShopInfoSlot shopInfoSlot)
    {
        gameObject.SetActive(true);
        _inputFieldShopName.text = string.Empty;
        _selectedShopInfoSlot = shopInfoSlot;
    }

    public void CreateNewShopInfo()
    {
        ShopInfo newShopInfo = new ShopInfo(_inputFieldShopName.text);
        OnCreateNewShopInfo?.Invoke(_selectedShopInfoSlot, newShopInfo, true);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
