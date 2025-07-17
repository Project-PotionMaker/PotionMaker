using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Sales : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _totalSalesTextUI;
    [SerializeField]
    private TextMeshProUGUI _dailySalesTextUI;

    [SerializeField]
    private Transform _slotContainer;
    private List<UI_SalesVolumeSlot> _salesVolumeSlotList;
    [SerializeField]
    private UI_SalesVolumeSlot _salesVolumeSlotPrefab;
    private void Start()
    {
        _salesVolumeSlotList = new List<UI_SalesVolumeSlot>();
        gameObject.SetActive(false);
    }
    public void Settle()
    {
        _totalSalesTextUI.text = SalesManager.Instance.Sales.TotalSales.ToString("N0");
        _dailySalesTextUI.text = SalesManager.Instance.Sales.DailySales.ToString("N0");

        int slotIndex = 0;
        foreach(EPotionType potionType in SalesManager.Instance.Sales.SalesVolumeDict.Keys)
        {
            if(slotIndex >= _salesVolumeSlotList.Count)
            {
                 UI_SalesVolumeSlot newSlot = GameObject.Instantiate(_salesVolumeSlotPrefab, _slotContainer);
                _salesVolumeSlotList.Add(newSlot);
            }
            _salesVolumeSlotList[slotIndex].Refresh(potionType);
            ++slotIndex;
        }

        for(int deleteIndex = _salesVolumeSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_SalesVolumeSlot deleteSlot = _salesVolumeSlotList[deleteIndex];

            _salesVolumeSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }



        gameObject.SetActive(true);
    }
}
