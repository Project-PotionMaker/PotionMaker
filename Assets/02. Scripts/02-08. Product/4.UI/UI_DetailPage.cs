using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField]
    private GameObject _layoutDescription;
    [SerializeField]
    private TextMeshProUGUI _InitialRentCostTextUI;
    [SerializeField]
    private TextMeshProUGUI _rentIncrementTextUI;

    private ProductData _productData;

    private void Start()
    {
        _buyButton.onClick.AddListener(OnClickBuyButton);
    }

    public void Refresh(ProductData productData)
    {
        _productData = productData;

        Sprite productSprite = ProductImageHelper.GetProductSprite(productData);
        _productImage.sprite = productSprite;
        _productNameTextUI.text = productData.Name;

        // TODO: 로컬라이제이션 연결로 수정
        _productDescriptionTextUI.text = productData.Description;
        _productPriceTextUI.text = productData.Price.ToString("N0");

        if(productData.ProductType == EProductType.HouseMoving)
        {
            _layoutDescription.SetActive(true);

            LayoutData layout = DataTable.Instance.GetLayoutData(productData.TargetTID);
            _InitialRentCostTextUI.text = layout.InitialRentCost.ToString("N0");
            _rentIncrementTextUI.text = layout.RentIncrement.ToString("N0");
        }
        else
        {
            _layoutDescription.SetActive(false);
        }
    }

    private void OnClickBuyButton()
    {
        if (_productData == null)
        {
            return;
        }

        ProductManager.Instance.CmdRequestBuy(_productData.ProductType, _productData.TID);
    }
}
