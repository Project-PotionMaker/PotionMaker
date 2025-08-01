using Mirror;
//using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ProductManager : NetworkBehaviourSingleton<ProductManager>
{
    private Dictionary<EProductType, List<Product>> _productListDict;
    public Dictionary<EProductType, List<ProductDTO>> ProductListDict =>
        _productListDict.ToDictionary
        (
            keyValuePair => keyValuePair.Key,
            keyValuePair => keyValuePair.Value.Select(product => product.ToDTO()).ToList()
        );
    private Delivery _delivery;
    private MovingHouse _movingHouse;
    protected override void Awake()
    {
        base.Awake();
        _delivery = new Delivery();
        _movingHouse = new MovingHouse();
        _movingHouse.InitMovingHouse(_delivery);
        _productListDict = new Dictionary<EProductType, List<Product>>()
        {
            { EProductType.Machine, new List<Product>() },
            { EProductType.Furniture, new List<Product>() },
            { EProductType.HouseMoving, new List<Product>() },
        };
    }
    private void Start()
    {
        Global.Instance.OnDataLoaded += InitProductManager;
        InitProductManager();
    }

    // 네트워크 매니저에서 처리
    //public override void OnJoinedRoom()
    //{
    //    InitProductManager();
    //}

    private void InitProductManager()
    {
        // 룸 관련 검사는 게임이 만들어지면 없애도 됨
        if (!Global.Instance.IsDataLoaded)
        {
            return;
        }

        LoadProductData();
        CmdRequestUpdateProducts();
    }

    private void LoadProductData()
    {
        ReadOnlyList<ProductData> productDataList = DataTable.Instance.GetProductDataList();
        foreach (ProductData productData in productDataList)
        {
            _productListDict[productData.ProductType].Add(new Product(productData));
        }
    }

    [Command(requiresAuthority =false)]
    public void CmdRequestBuy(EProductType productType, int productID, NetworkConnectionToClient sender = null)
    {
        Product product = _productListDict[productType].Find(product => product.Data.TID == productID);

        bool result = TryBuy(product);
        TargetShowResultUI(sender, result);
    }

    [Server]
    private bool TryBuy(Product product)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(TryBuy)}() is server-only. Use {nameof(CmdRequestBuy)}() from client.");
        }
        if (!CurrencyManager.Instance.TrySubtractCurrency(product.Data.Price))
        {
            return false;
        }
        switch (product.Data.ProductType)
        {
            case EProductType.Machine:
            case EProductType.Furniture:
            {
                // Todo: productId를 통한 addressable 호출

                Debug.Log("[구매]\n" +
                          $"상품번호: {product.Data.TID}\n" +
                          $"상품이름: {product.Data.Name}\n" +
                          $"상품가격: {product.Data.Price}");

                _delivery.DeliverStructure(product.Data.TargetTID, EAreaType.FrontYard);
                break;
            }
            case EProductType.HouseMoving:
            {
                _movingHouse.MoveHouse(product.Data.TargetTID);
                break;
            }
            default:
            {
                break;
            }
        }
        return true;
    }

    [TargetRpc]
    public void TargetShowResultUI(NetworkConnection taraget, bool result)
    {
        // Todo: 팝업매니저를 통한 구매 성공 여부 팝업?
        Debug.Log($"구매 결과: {result}");
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestUnlock(int productID)
    {
        Unlock(productID);
    }

    [Server]
    private void Unlock(int productTID)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(Unlock)}() is server-only. Use {nameof(CmdRequestUnlock)}() from client.");
        }

        Product targetProduct = _productListDict.SelectMany(keyValuePair => keyValuePair.Value).FirstOrDefault(product => product.Data.TID == productTID);
        targetProduct.Unlock();
        UpdateProduct(targetProduct.Data.TID, targetProduct.IsUnlocked);
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestUpdateProducts()
    {
        UpdateProducts();
    }

    [Server]
    private void UpdateProducts()
    {
        if (!isServer)
        {
            throw new InvalidOperationException("UpdateProducts() is server-only. Use CmdRequestUpdateProducts() from client.");
        }

        List<ProductDTO> targetProductList = ProductListDict.SelectMany(keyValuePair => keyValuePair.Value).ToList();
        foreach (ProductDTO productDTO in targetProductList)
        {
            UpdateProduct(productDTO.Data.TID, productDTO.IsUnlocked);
        }
    }

    [ClientRpc]
    public void UpdateProduct(int productTID, bool isUnlocked)
    {
        Product targetProduct = _productListDict.SelectMany(keyValuePair => keyValuePair.Value).FirstOrDefault(product => product.Data.TID == productTID);
        targetProduct.SetProduct(isUnlocked);
    }
}
