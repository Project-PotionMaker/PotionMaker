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

    public void Refresh(ProductDTO productDTO)
    {
        gameObject.SetActive(false);
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
        _productPriceTextUI.text = productDTO.Data.Price.ToString("N0");
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(productDTO));
        gameObject.SetActive(true);
    }
}
