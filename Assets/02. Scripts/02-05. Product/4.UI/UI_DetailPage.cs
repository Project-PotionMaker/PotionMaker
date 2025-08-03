using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DetailPage : MonoBehaviour
{
    private const string ASSET_PREFIX = "Image_Product_";

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

    public async Task Refresh(ProductDTO productDTO)
    {
        _productImage.sprite = await AssetManager.Instance.LoadAsset<Sprite>($"{ASSET_PREFIX}{productDTO.Data.TID}");
        _productNameTextUI.text = productDTO.Data.Name;

        // TODO: 로컬라이제이션 연결로 수정
        _productDescriptionTextUI.text = productDTO.Data.Description;
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(() => { ProductManager.Instance.CmdRequestBuy(productDTO.Data.ProductType, productDTO.Data.TID); });

        if(productDTO.Data.ProductType == EProductType.HouseMoving)
        {
            _layoutDescription.SetActive(true);

            LayoutData layout = DataTable.Instance.GetLayoutData(productDTO.Data.TargetTID);
            _InitialRentCostTextUI.text = layout.InitialRentCost.ToString("N0");
            _rentIncrementTextUI.text = layout.RentIncrement.ToString("N0");
        }
        else
        {
            _layoutDescription.SetActive(false);
        }
    }
}
