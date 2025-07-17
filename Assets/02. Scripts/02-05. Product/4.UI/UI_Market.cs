using System.Collections.Generic;
using UnityEngine;

public class UI_Market : MonoBehaviour
{

    [SerializeField]
    private Transform _slotContainer;
    private List<UI_ProductSlot> _productSlotList;
    [SerializeField]
    private UI_ProductSlot _productSlotPrefab;

    public void OnMachineButtonClicked() => OnProductTypeButtonClicked(EProductType.Machine);
    public void OnFurnitureButtonClicked() => OnProductTypeButtonClicked(EProductType.Furniture);
    public void OnHouseMovingButtonClicked() => OnProductTypeButtonClicked(EProductType.HouseMoving);

    private void Awake()
    {
        _productSlotList = new List<UI_ProductSlot>();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        OnProductTypeButtonClicked(EProductType.Machine);
    }
    public void OnProductTypeButtonClicked(EProductType productType)
    {
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
                _productSlotList.Add(newSlot);
            }
            _productSlotList[slotIndex].Refresh(productDTO);
            ++slotIndex;
        }

        for (int deleteIndex = _productSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_ProductSlot deleteSlot = _productSlotList[deleteIndex];

            _productSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }
    }
}
