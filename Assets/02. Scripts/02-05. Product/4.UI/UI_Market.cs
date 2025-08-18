using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Market : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _coinValueTextUI;
    [SerializeField]
    private Transform _slotContainer;
    private List<UI_ProductSlot> _productSlotList;
    [SerializeField]
    private UI_ProductSlot _productSlotPrefab;

    [SerializeField]
    private UI_DetailPage _detailPage;

    public void OnMachineButtonClicked() => OnProductTypeButtonClicked(EProductType.Machine);
    public void OnFurnitureButtonClicked() => OnProductTypeButtonClicked(EProductType.Furniture);
    public void OnHouseMovingButtonClicked() => OnProductTypeButtonClicked(EProductType.HouseMoving);

    private void Awake()
    {
        _productSlotList = new List<UI_ProductSlot>();
    }

    private void OnEnable()
    {
        RefreshCoin();
        OnProductTypeButtonClicked(EProductType.Machine);
        //RefreshDetailPage(_productSlotList[0])

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(EPopupAudioType.Market);
        }
    }

    private void Start()
    {
        CurrencyManager.OnDataChanged += RefreshCoin;
        gameObject.SetActive(false);
    }

    public void OnProductTypeButtonClicked(EProductType productType)
    {
        bool isDetailPageRefreshed = false;
        int slotIndex = 0;
        foreach(ProductDTO productDTO in ProductManager.Instance.ProductListDict[productType])
        {
            if (!productDTO.IsUnlocked)
            {
                continue;
            }
            if(slotIndex >= _productSlotList.Count)
            {
                UI_ProductSlot newSlot = Instantiate(_productSlotPrefab, _slotContainer);
                newSlot.OnSlotClicked += RefreshDetailPage;
                _productSlotList.Add(newSlot);
            }
            _productSlotList[slotIndex].Refresh(productDTO);
            ++slotIndex;

            if (!isDetailPageRefreshed)
            {
                isDetailPageRefreshed = true;
                _detailPage.Refresh(productDTO);
            }
        }

        for (int deleteIndex = _productSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_ProductSlot deleteSlot = _productSlotList[deleteIndex];
            deleteSlot.OnSlotClicked -= RefreshDetailPage;
            _productSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }
    }

    public void RefreshDetailPage(ProductDTO productDTO)
    {
        _detailPage.Refresh(productDTO);
    }

    public void RefreshCoin()
    {
        _coinValueTextUI.text = CurrencyManager.Instance.Coin.Value.ToString("N0");
    }
}
