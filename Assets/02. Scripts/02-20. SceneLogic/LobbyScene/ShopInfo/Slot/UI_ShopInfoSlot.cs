using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UI_ShopInfoSlot : MonoBehaviour
{
    [Header("Project")]
    [SerializeField]
    private Button _buttonCreateNewShop;

    [SerializeField]
    private Button _buttonShopName;

    [SerializeField]
    private Button _buttonDelete;

    [SerializeField]
    private TextMeshProUGUI _textCreateNewShop;
    public TextMeshProUGUI TextCreateNewShop => _textCreateNewShop;

    [SerializeField]
    private TextMeshProUGUI _textShopName;
    public TextMeshProUGUI TextShopName => _textShopName;

    [Header("Project")]
    private ShopInfoSlot _shopInfoSlot;

    private void Start()
    {
        _shopInfoSlot = GetComponent<ShopInfoSlot>();
        _shopInfoSlot.OnShopInfoCreated += Refresh;
        _shopInfoSlot.OnShopInfoDeleted += RefreshOnDelete;
    }

    public void Refresh(ShopInfo shopInfo)
    {
        if (shopInfo != null)
        {
            _textShopName.text = shopInfo.ShopName;
            _buttonCreateNewShop.gameObject.SetActive(false);
            _buttonShopName.gameObject.SetActive(true);
        }
    }

    public void RefreshOnDelete()
    {
        _textShopName.text = string.Empty;
        _buttonCreateNewShop.gameObject.SetActive(true);
        _buttonShopName.gameObject.SetActive(false);
    }

    public void RefreshSelected()
    {
        _buttonShopName.gameObject.SetActive(true);
    }

    public void RefreshUnSelected()
    {
        _buttonShopName.gameObject.SetActive(false);
    }

}
