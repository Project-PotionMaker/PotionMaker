using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_ProductSlot:MonoBehaviour
{
    public event Action<ProductData> OnSlotClicked;

    [SerializeField]
    private Image _productImage;
    [SerializeField]
    private TextMeshProUGUI _productNameTextUI;
    [SerializeField]
    private TextMeshProUGUI _productPriceTextUI;
    [SerializeField]
    private Button _slotButton;

    [SerializeField]
    private CanvasGroup _lockPanel;

    private ProductData _productData;

    private void Start()
    {
        _slotButton.onClick.AddListener(OnClickSlotButtion);
    }

    public void Refresh(ProductDTO productDTO)
    {
        gameObject.SetActive(false);
        _productData = productDTO.Data;

        Sprite productSprite = ProductImageHelper.GetProductSprite(_productData);
        _productImage.sprite = productSprite;
        _productNameTextUI.text = productDTO.Data.Name;
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");

        _lockPanel.alpha = productDTO.IsUnlocked ? 0 : 1;
        _slotButton.enabled = productDTO.IsUnlocked;

        gameObject.SetActive(true);
    }

    private void OnClickSlotButtion()
    {
        if (_productData == null)
        {
            return;
        }

        OnSlotClicked?.Invoke(_productData);
    }
}
