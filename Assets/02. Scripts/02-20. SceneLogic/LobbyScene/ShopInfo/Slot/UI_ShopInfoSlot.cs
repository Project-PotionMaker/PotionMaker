using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopInfoSlot : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private Button _buttonCreateNewShop;

    [SerializeField]
    private Button _buttonShopName;

    [SerializeField]
    private Button _buttonDelete;

    [SerializeField]
    private TextMeshProUGUI _textCreateNewShop;

    [SerializeField]
    private TextMeshProUGUI _textShopName;

    private ShopInfoSlot _shopInfoSlot;

    private void Start()
    {
        _shopInfoSlot = GetComponent<ShopInfoSlot>();
        _shopInfoSlot.OnShopInfoCreated += RefreshOnCreated;
        _shopInfoSlot.OnShopInfoSelected += RefreshOnSelected;
        _shopInfoSlot.OnShopInfoUnSelected += RefreshOnUnSelected;
        _shopInfoSlot.OnShopInfoDeleted += RefreshOnDeleted;
    }

    public void RefreshOnCreated(ShopInfo shopInfo)
    {
        if (shopInfo != null)
        {
            _textShopName.text = shopInfo.ShopName;
            _buttonCreateNewShop.gameObject.SetActive(false);
            _buttonShopName.gameObject.SetActive(true);
            _buttonDelete.gameObject.SetActive(true);
        }
    }

    public void RefreshOnDeleted()
    {
        _textShopName.text = string.Empty;
        _buttonCreateNewShop.gameObject.SetActive(true);
        _buttonShopName.gameObject.SetActive(false);
    }

    public void RefreshOnSelected()
    {
        ChangeButtonColor(_buttonShopName, Color.white);
        _buttonDelete.gameObject.SetActive(true);
    }

    public void RefreshOnUnSelected()
    {
        ChangeButtonColor(_buttonShopName, Color.grey);
        _buttonDelete.gameObject.SetActive(false);
    }

    private void ChangeButtonColor(Button button, Color targetColor)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = targetColor;
        button.colors = colorBlock;
    }
}
