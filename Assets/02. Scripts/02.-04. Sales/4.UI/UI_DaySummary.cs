using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DaySummary : MonoBehaviour
{
    private const string POSITIVE_COLOR = "#65BC04";
    private const string NEGATIVE_COLOR = "#CB0000";
    private const string DEFAULT_COLOR = "#000000";

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

    [Header("결과창")]
    [SerializeField]
    private UI_SuccessSummary _successPanel;
    [SerializeField]
    private UI_GameOverSummary _gameOverPanel;

    private void Start()
    {
        _salesVolumeSlotList = new List<UI_SalesVolumeSlot>();
        PhaseManager.Instance.PhaseDictionary[EPhaseType.EndingPhase].OnPhaseEntered += OnEndingPhaseStarted;
        SalesManager.Instance.OnSummaryReady += ShowSummary;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PhaseManager.Instance.IsGameOver)
            {
                _gameOverPanel.ShowSummary();
                AudioManager.Instance.PlaySFX(EPhaseAudioType.EndingPhaseSuccess);
                
            }
            else
            {
                _successPanel.ShowSummary();
                AudioManager.Instance.PlaySFX(EPhaseAudioType.EndingPhaseFailure);
            }
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
        _deathCountView.fillAmount = (float)(PhaseManager.Instance.DeathCount) / PhaseManager.Instance.MaxDeathCount;

        // 일 매출
        _dailySalesTextUI.text = SalesManager.Instance.Sales.DailySales.ToString("N0");

        // 방세
        _rentPanel.SetActive(RentManager.Instance.Rent.IsRentDay);
        _rentTextUI.text = $"-{RentManager.Instance.Rent.CurrentRentCost.ToString("N0")}";

        // 평판
        // max value도 받아오도록 수정
        float currentReputation = ReputationManager.Instance.Reputation.Value;
        _reputationRateView.fillAmount = currentReputation / 5;
        _currentReputationTextUI.text = currentReputation.ToString("F1");

        float reputationDifference = ReputationManager.Instance.Reputation.Difference;
        string color = reputationDifference >= 0 ? POSITIVE_COLOR : NEGATIVE_COLOR;
        _deltaReputationTextUI.text = $"<color={color}>{reputationDifference.ToString("+0.0;-0.0;+0.0")}</color>";

        int currentCurrency = CurrencyManager.Instance.Coin.Value;
        if(PhaseManager.Instance.IsGameOver == true && PhaseManager.Instance.DeathCount > 0)
        {
            currentCurrency -= RentManager.Instance.Rent.CurrentRentCost;
            ColorUtility.TryParseHtmlString(NEGATIVE_COLOR, out Color negativeColor);
            _currentCurrencyTextUI.color = negativeColor;
        }
        else
        {
            ColorUtility.TryParseHtmlString(DEFAULT_COLOR, out Color defaultColor);
            _currentCurrencyTextUI.color = defaultColor;
        }
            _currentCurrencyTextUI.text = currentCurrency.ToString("N0");

        gameObject.SetActive(true);
    }
}
