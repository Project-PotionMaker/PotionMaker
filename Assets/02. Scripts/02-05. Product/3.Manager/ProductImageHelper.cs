using UnityEngine;

public static class ProductImageHelper
{
    public static Sprite GetProductSprite(ProductData productData)
    {
        Sprite productSprite;
        switch (productData.ProductType)
        {
            case EProductType.Machine:
            {
                int machineTID = DataTable.Instance.GetStructureData(productData.TargetTID).TypeTID;
                productSprite = ImageManager.Instance.GetImage<MachineData>(machineTID);
                break;
            }
            case EProductType.Furniture:
            {
                int furnitureTID = DataTable.Instance.GetStructureData(productData.TargetTID).TypeTID;
                productSprite = ImageManager.Instance.GetImage<FurnitureData>(furnitureTID);
                break;
            }
            case EProductType.HouseMoving:
            {
                int layoutTID = productData.TargetTID;
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
