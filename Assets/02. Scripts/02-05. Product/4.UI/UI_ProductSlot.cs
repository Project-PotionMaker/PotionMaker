using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProductSlot:MonoBehaviour
{
    public event Action<ProductDTO> OnSlotClicked;

    [SerializeField]
    private Image _productImage;
    [SerializeField]
    private TextMeshProUGUI _productNameTextUI;
    [SerializeField]
    private TextMeshProUGUI _productPriceTextUI;
    [SerializeField]
    private Button _slotButton;

    public void Refresh(ProductDTO productDTO)
    {
        _productNameTextUI.text = productDTO.Data.Name;
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(productDTO));
    }
}
