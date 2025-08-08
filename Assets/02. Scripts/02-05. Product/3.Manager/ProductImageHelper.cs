using UnityEngine;

public static class ProductImageHelper
{
    public static Sprite GetProductSprite(ProductDTO productDTO)
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
        return productSprite;
    }
}
