using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOverSummary : MonoBehaviour
{
    private const string POSITIVE_COLOR = "#65BC04";
    private const string NEGATIVE_COLOR = "#CB0000";

    [SerializeField]
    private Transform[] _slotContainer;
    private List<UI_GameOverVolumeSlot> _salesVolumeSlotList;
    [SerializeField]
    private UI_GameOverVolumeSlot _salesVolumeSlotPrefab;

    [Header("영업기록")]
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyTextUI;
    [SerializeField]
    private TextMeshProUGUI _totalSalesTextUI;
    [SerializeField]
    private TextMeshProUGUI _totalDay;

    [SerializeField]
    private TextMeshProUGUI _totalPotions;

    private void Start()
    {
        _salesVolumeSlotList = new List<UI_GameOverVolumeSlot>();
        gameObject.SetActive(false);
    }
    private void Update()
    {
    }
    public void ShowSummary()
    {
        // 포션 별 판매 기록
        int slotIndex = 0;
        foreach (int potionTID in SalesManager.Instance.Sales.TotalSalesVolumeDict.Keys)
        {
            if (slotIndex >= _salesVolumeSlotList.Count)
            {
                UI_GameOverVolumeSlot newSlot = GameObject.Instantiate(_salesVolumeSlotPrefab, _slotContainer[slotIndex%2]);
                _salesVolumeSlotList.Add(newSlot);
            }
            _salesVolumeSlotList[slotIndex].Refresh(potionTID, false);
            ++slotIndex;
        }
        int currentCurrency = CurrencyManager.Instance.Coin.Value;
        _currentCurrencyTextUI.text = currentCurrency.ToString("N0");

        int totalSales = SalesManager.Instance.Sales.TotalSales;
        _totalSalesTextUI.text = totalSales.ToString("N0");
        _totalDay.text = $"{PhaseManager.Instance.Day} 일";

        _totalPotions.text = SalesManager.Instance.Sales.GetTotalSalesVolume().ToString("N0");


        gameObject.SetActive(true);
    }
}
