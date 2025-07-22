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

    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
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
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_TryBuy), RpcTarget.MasterClient, productType, productID);
            return;
        }

        Product product = _productListDict[productType].Find(product => product.Data.TID == productID);
        
        RPC_TryBuy(product, new PhotonMessageInfo(PhotonNetwork.LocalPlayer, PhotonNetwork.ServerTimestamp, _photonView));
    }

    [PunRPC]
    public void RPC_TryBuy(Product product, PhotonMessageInfo info)
    {
        if (CurrencyManager.Instance.TrySubtractCurrency(product.Data.Price))
        {
            _photonView.RPC(nameof(RPC_ShowResultUI), info.Sender, false);
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

                GameObject structure = StructureManager.Instance.CreateStructure(product.Data.StructureTID);
                structure.transform.position = Vector3.zero;
                break;
            }
            case EProductType.HouseMoving:
            {
                // Todo: 씬 전환
                break;
            }
            default:
            {
                break;
            }
        }
        _photonView.RPC(nameof(RPC_ShowResultUI), info.Sender, true);
    }

    [PunRPC]
    public void RPC_ShowResultUI(bool result)
    {
        // Todo: 팝업매니저를 통한 구매 성공 여부 팝업?
        Debug.Log($"구매 결과: {result}");
    }

    public void RequestUnlock(int productID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_Unlock), RpcTarget.MasterClient, productID);
            return;
        }
        RPC_Unlock(productID);
    }

    [PunRPC]
    public void RPC_Unlock(int productTID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Only the Master Client may Uunlock products directly. Use 'RequestUnlock' instead.");
        }

        Product targetProduct = _productListDict.SelectMany(keyValuePair => keyValuePair.Value).FirstOrDefault(product => product.Data.TID == productTID);
        targetProduct.Unlock();
        _photonView.RPC(nameof(RPC_SetProduct), RpcTarget.Others, targetProduct.Data.TID, targetProduct.IsUnlocked);
    }

    [PunRPC]
    public void RPC_SetProduct(int productTID, bool isUnlocked, PhotonMessageInfo info)
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
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_UpdateProducts), RpcTarget.MasterClient);
            return;
        }
        RPC_UpdateProducts();
    }

    [PunRPC]
    public void RPC_UpdateProducts()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Only the Master Client may Update products directly. Use 'RequestUpdateProducts' instead.");
        }

        List<ProductDTO> targetProductList = ProductListDict.SelectMany(keyValuePair => keyValuePair.Value).ToList();
        foreach (ProductDTO productDTO in targetProductList)
        {
            _photonView.RPC(nameof(RPC_SetProduct), RpcTarget.Others, productDTO.Data.TID, productDTO.IsUnlocked);
        }
    }
}
