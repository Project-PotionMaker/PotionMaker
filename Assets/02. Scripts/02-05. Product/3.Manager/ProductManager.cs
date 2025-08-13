using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    protected void Awake()
    {
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        Global.Instance.OnDataLoaded += LoadProductData;
        LoadProductData();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드되면 PotionHouse를 찾아 초기화를 시도합니다.
        InitProductManager();
    }

    private void InitProductManager()
    {
        if (UnlockManager.Instance.NewUnlockedTIDDict != null)
        {
            UnlockProducts();
        }
        else
        {
            UnlockManager.Instance.OnListUpdated -= UnlockProducts;
            UnlockManager.Instance.OnListUpdated += UnlockProducts;
        }
    }

    private void UnlockProducts()
    {
        if (UnlockManager.Instance.NewUnlockedTIDDict.TryGetValue(EUnlockType.Structure, out ReadOnlyList<int> unlockedStructureTIDList))
        {
            foreach(int unlockedStructureTID in unlockedStructureTIDList)
            {
                Debug.Log(NetworkClient.ready);
                Unlock(DataTable.Instance.GetStructureData(unlockedStructureTID).ProductTID);
            }
        }
        var nextTierLayoutDataList = DataTable.Instance.GetLayoutDataList().Where(data => data.Tier == PotionHouse.Instance.PotionHouseTier + 1);
        foreach(LayoutData layoutData in nextTierLayoutDataList)
        {
            Unlock(layoutData.ProductTID);
        }
    }
    private void LoadProductData()
    {
        if (!Global.Instance.IsDataLoaded)
        {
            return;
        }
        ReadOnlyList<ProductData> productDataList = DataTable.Instance.GetProductDataList();
        foreach (ProductData productData in productDataList)
        {
            _productListDict[productData.ProductType].Add(new Product(productData));
        }
    }

    [Command(requiresAuthority = false)]
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

    private void Unlock(int productTID)
    {
        Product targetProduct = _productListDict.SelectMany(keyValuePair => keyValuePair.Value).FirstOrDefault(product => product.Data.TID == productTID);
        targetProduct.Unlock();
    }
}
