using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProductSlot:MonoBehaviour
{
    private const string ASSET_PREFIX = "Image_Product_";
    public event Action<ProductDTO> OnSlotClicked;

    [SerializeField]
    private Image _productImage;
    [SerializeField]
    private TextMeshProUGUI _productNameTextUI;
    [SerializeField]
    private TextMeshProUGUI _productPriceTextUI;
    [SerializeField]
    private Button _slotButton;

    public async void Refresh(ProductDTO productDTO)
    {
        gameObject.SetActive(false);
        _productImage.sprite = await AssetManager.Instance.LoadAsset<Sprite>($"{ASSET_PREFIX}{productDTO.Data.TID}");
        _productNameTextUI.text = productDTO.Data.Name;
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(productDTO));
        gameObject.SetActive(true);
    }
}
