using UnityEngine;

public class ProductDTO
{
    public readonly ProductData Data;
    public readonly bool IsUnlocked;

    public ProductDTO(Product product)
    {
        Data = product.Data;
        IsUnlocked = product.IsUnlocked;
    }
}
