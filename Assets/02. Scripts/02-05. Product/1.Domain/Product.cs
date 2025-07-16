using System;

public class Product
{
    private ProductData _data;
    public ProductData Data => _data;

    private bool _isUnlocked;
    public bool IsUnlocked => _isUnlocked;

    public Product(ProductData data, bool isUnlocked = false)
    {
        // 유효성 검사 예외 처리는 ProductData에서 해야함
        _data = data;
        _isUnlocked = isUnlocked; ;
    }

    public void Unlock()
    {
        _isUnlocked = true;
    }

    public void SetProduct(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }
    public ProductDTO ToDTO()
    {
        return new ProductDTO(this);
    }
}
