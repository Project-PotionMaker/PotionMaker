using System.Threading.Tasks;
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

    [SerializeField]
    private GameObject _layoutDescription;
    [SerializeField]
    private TextMeshProUGUI _InitialRentCostTextUI;
    [SerializeField]
    private TextMeshProUGUI _rentIncrementTextUI;

    public void Refresh(ProductDTO productDTO)
    {
        Sprite productSprite;
        switch (productDTO.Data.ProductType)
        {
            case EProductType.Machine:
            {
                int machineTID = DataTable.Instance.GetStructureData(productDTO.Data.TargetTID).TypeTID;
                productSprite = ImageManager.Instance.GetImage<MachineData>(machineTID);
                break;
            }
            case EProductType.Furniture:
            {
                int furnitureTID = DataTable.Instance.GetStructureData(productDTO.Data.TargetTID).TypeTID;
                productSprite = ImageManager.Instance.GetImage<FurnitureData>(furnitureTID);
                break;
            }
            case EProductType.HouseMoving:
            {
                int layoutTID = productDTO.Data.TargetTID;
                productSprite = ImageManager.Instance.GetImage<LayoutData>(layoutTID);
                break;
            }
            default:
            {
                productSprite = null;
                break;
            }
        }
        _productImage.sprite = productSprite;
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
