using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ProductManager : MonoBehaviourPunCallbacksSingleton<ProductManager>
{
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

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
        _photonView = GetComponent<PhotonView>();
        _delivery = new Delivery();
        _movingHouse = new MovingHouse();
        _movingHouse.InitMovingHouse(_delivery);
    }
    private void Start()
    {
        Global.Instance.OnDataLoaded += InitProductManager;
        InitProductManager();
    }

    // 없애도 됨
    public override void OnJoinedRoom()
    {
        InitProductManager();
    }

    private void InitProductManager()
    {
        // 룸 관련 검사는 게임이 만들어지면 없애도 됨
        if (!PhotonNetwork.InRoom || !Global.Instance.IsDataLoaded)
        {
            return;
        }

        _productListDict = new Dictionary<EProductType, List<Product>>()
        {
            { EProductType.Machine, new List<Product>() },
            { EProductType.Furniture, new List<Product>() },
            { EProductType.HouseMoving, new List<Product>() },
        };

        LoadProductData();
        RequestUpdateProducts();
    }

    private void LoadProductData()
    {
        ReadOnlyList<ProductData> productDataList = DataTable.Instance.GetProductDataList();
        foreach (ProductData productData in productDataList)
        {
            _productListDict[productData.ProductType].Add(new Product(productData));
        }
    }

    public void RequestBuy(EProductType productType, int productID)
    {
        _photonView.RPC(nameof(RequestBuy), RpcTarget.MasterClient, productType, productID);
    }
    [PunRPC]
    public void RequestBuy(EProductType productType, int productID, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RequestBuy), RpcTarget.MasterClient, productType, productID);
            return;
        }

        Product product = _productListDict[productType].Find(product => product.Data.TID == productID);
        bool result = TryBuy(product);

        _photonView.RPC(nameof(ShowResultUI), info.Sender, result);
    }

    private bool TryBuy(Product product)
    {
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

                _delivery.DeliverStructure(product.Data.TargetTID, EAreaType.FrontYard ,0, out int dummy);
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

    [PunRPC]
    public void ShowResultUI(bool result)
    {
        // Todo: 팝업매니저를 통한 구매 성공 여부 팝업?
        Debug.Log($"구매 결과: {result}");
    }

    [PunRPC]
    public void RequestUnlock(int productID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RequestUnlock), RpcTarget.MasterClient, productID);
            return;
        }
        Unlock(productID);
    }

    private void Unlock(int productTID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Product targetProduct = _productListDict.SelectMany(keyValuePair => keyValuePair.Value).FirstOrDefault(product => product.Data.TID == productTID);
        targetProduct.Unlock();
        _photonView.RPC(nameof(SetProduct), RpcTarget.Others, targetProduct.Data.TID, targetProduct.IsUnlocked);
    }

    [PunRPC]
    public void SetProduct(int productTID, bool isUnlocked, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            throw new InvalidOperationException("Product must be Set by the Master Client");
        }
        Product targetProduct = _productListDict.SelectMany(keyValuePair => keyValuePair.Value).FirstOrDefault(product => product.Data.TID == productTID);
        targetProduct.SetProduct(isUnlocked);
    }

    public void RequestUpdateProducts()
    {
        _photonView.RPC(nameof(RequestUpdateProducts), RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RequestUpdateProducts(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RequestUpdateProducts), RpcTarget.MasterClient);
            return;
        }

        List<ProductDTO> targetProductList = ProductListDict.SelectMany(keyValuePair => keyValuePair.Value).ToList();
        foreach (ProductDTO productDTO in targetProductList)
        {
            _photonView.RPC(nameof(SetProduct), info.Sender, productDTO.Data.TID, productDTO.IsUnlocked);
        }
    }
}
