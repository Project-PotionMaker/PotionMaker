using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DaySummary : MonoBehaviour
{
    private const string POSITIVE_COLOR = "65BC04";
    private const string NEGATIVE_COLOR = "CB0000";

    [SerializeField]
    private Transform _slotContainer;
    private List<UI_SalesVolumeSlot> _salesVolumeSlotList;
    [SerializeField]
    private UI_SalesVolumeSlot _salesVolumeSlotPrefab;

    [Header("데스카운트")]
    [SerializeField]
    private Image _deathCountView;

    [Header("일매출")]
    [SerializeField]
    private TextMeshProUGUI _dailySalesTextUI;

    [Header("방세")]
    [SerializeField]
    private GameObject _rentPanel;
    [SerializeField]
    private TextMeshProUGUI _rentTextUI;

    [Header("평판")]
    [SerializeField]
    private Image _reputationRateView;
    [SerializeField]
    private TextMeshProUGUI _currentReputationTextUI;
    [SerializeField]
    private TextMeshProUGUI _deltaReputationTextUI;

    [Header("자산")]
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyTextUI;

    private void Start()
    {
        _salesVolumeSlotList = new List<UI_SalesVolumeSlot>();
        // 엔딩페이즈 구독 += OnEndingPhaseStarted
        SalesManager.Instance.OnSummaryReady += ShowSummary;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        // 포션 별 판매 기록
        int slotIndex = 0;
        foreach (int potionTID in SalesManager.Instance.Sales.DailySalesVolumeDict.Keys)
        {
            if (slotIndex >= _salesVolumeSlotList.Count)
            {
                UI_SalesVolumeSlot newSlot = GameObject.Instantiate(_salesVolumeSlotPrefab, _slotContainer);
                _salesVolumeSlotList.Add(newSlot);
            }
            _salesVolumeSlotList[slotIndex].Refresh(potionTID, false);
            ++slotIndex;
        }

        for (int deleteIndex = _salesVolumeSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_SalesVolumeSlot deleteSlot = _salesVolumeSlotList[deleteIndex];

            _salesVolumeSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }

        // 데스카운트
        _deathCountView.fillAmount = 1 - (PhaseManager.Instance.DeathCount) / PhaseManager.Instance.MaxDeathCount;

        // 일 매출
        _dailySalesTextUI.text = SalesManager.Instance.Sales.DailySales.ToString("N0");

        // 방세
        _rentPanel.SetActive(RentManager.Instance.Rent.IsRentDay);
        _rentTextUI.text = $"-{RentManager.Instance.Rent.CurrentRentCost.ToString("N0")}";

        // 평판
        // max value도 받아오도록 수정
        float currentReputation = ReputationManager.Instance.Reputation.Value;
        _reputationRateView.fillAmount = currentReputation / 5;
        _currentReputationTextUI.text = currentReputation.ToString();
        //float deltaReputationRate = ReputationManager.Instance.DeltaReputationRate;
        //string color = deltaReputationRate >= 0 ? POSITIVE_COLOR : NEGATIVE_COLOR;
        //_deltaReputation.text = $"<color={color}>{_deltaReputationRate}</color>;


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
