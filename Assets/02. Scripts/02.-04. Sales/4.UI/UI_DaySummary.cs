using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DaySummary : MonoBehaviour
{
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
        // 엔딩페이즈 구독 += OnEndingPhaseStarted
        SalesManager.Instance.OnSummaryReady += ShowSummary;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
    public void OnEndingPhaseStarted()
    {
        SalesManager.Instance.CmdRequestUpdateSales(isForSummary: true);
    }
    public void ShowSummary()
    {
        _dailySalesTextUI.text = SalesManager.Instance.Sales.DailySales.ToString("N0");

        int slotIndex = 0;
        foreach (EPotionType potionType in SalesManager.Instance.Sales.DailySalesVolumeDict.Keys)
        {
            if (slotIndex >= _salesVolumeSlotList.Count)
            {
                UI_SalesVolumeSlot newSlot = GameObject.Instantiate(_salesVolumeSlotPrefab, _slotContainer);
                _salesVolumeSlotList.Add(newSlot);
            }
            _salesVolumeSlotList[slotIndex].Refresh(potionType, false);
            ++slotIndex;
        }

        for (int deleteIndex = _salesVolumeSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_SalesVolumeSlot deleteSlot = _salesVolumeSlotList[deleteIndex];

            _salesVolumeSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }
        gameObject.SetActive(true);
    }
}
