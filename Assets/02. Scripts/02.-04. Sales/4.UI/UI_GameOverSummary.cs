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

    [Header("자산")]
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyTextUI;

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
        foreach (int potionTID in SalesManager.Instance.Sales.DailySalesVolumeDict.Keys)
        {
            if (slotIndex >= _salesVolumeSlotList.Count)
            {
                UI_GameOverVolumeSlot newSlot = GameObject.Instantiate(_salesVolumeSlotPrefab, _slotContainer[slotIndex%2]);
                _salesVolumeSlotList.Add(newSlot);
            }
            _salesVolumeSlotList[slotIndex].Refresh(potionTID, false);
            ++slotIndex;
        }

        for (int deleteIndex = _salesVolumeSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_GameOverVolumeSlot deleteSlot = _salesVolumeSlotList[deleteIndex];

            _salesVolumeSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }


        // 자산
        // 미리 방세 빼는 로직이 들어가있어야함? 아니면 표시만?
        int currentCurrency = CurrencyManager.Instance.Coin.Value;
        if (RentManager.Instance.Rent.IsRentDay)
        {
            currentCurrency -= RentManager.Instance.Rent.CurrentRentCost;
        }
        _currentCurrencyTextUI.text = currentCurrency.ToString("N0");
        gameObject.SetActive(true);
    }
}
