using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DetailPage : MonoBehaviour
{
    [SerializeField]
    private Image _productImage;
    [SerializeField]
    private TextMeshProUGUI _productNameTextUI;
    [SerializeField]
    private TextMeshProUGUI _productDescriptionTextUI;
    [SerializeField]
    private TextMeshProUGUI _productPriceTextUI;
    [SerializeField]
    private Button _buyButton;

    
    public void Refresh(ProductDTO productDTO)
    {
        //_productImage
        _productNameTextUI.text = productDTO.Data.Name;
        //_productDescriptionTextUI.text = productDTO.Data.
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(() => { ProductManager.Instance.RequestBuy(productDTO.Data.ProductType, productDTO.Data.TID); });
    }
}
