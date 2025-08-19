using System;
using System.Threading.Tasks;
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

    [SerializeField]
    private CanvasGroup _lockPanel;

    public void Refresh(ProductDTO productDTO)
    {
        gameObject.SetActive(false);
        Sprite productSprite = ProductImageHelper.GetProductSprite(productDTO);
        _productImage.sprite = productSprite;
        _productNameTextUI.text = productDTO.Data.Name;
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(productDTO));

        _lockPanel.alpha = productDTO.IsUnlocked ? 0 : 1;
        _slotButton.enabled = productDTO.IsUnlocked;

        gameObject.SetActive(true);
    }
}
