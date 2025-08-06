using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SuccessSummary : MonoBehaviour
{
    private const string POSITIVE_COLOR = "#65BC04";
    private const string NEGATIVE_COLOR = "#CB0000";

    [Header("영업기록")]
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyTextUI;
    [SerializeField]
    private TextMeshProUGUI _totalSalesTextUI;
    [SerializeField]
    private TextMeshProUGUI _totalDay;

    [Header("다음납부")]
    [SerializeField]
    private TextMeshProUGUI _nextRentDay;
    [SerializeField]
    private TextMeshProUGUI _nextRentCost;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        //TODO : 투표시스템
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.SetActive(false);
            PhaseManager.Instance.TransitionPhase(EPhaseType.PreparingPhase);
        }
    }
    public void ShowSummary()
    {
        // 미리 방세 빼는 로직이 들어가있어야함? 아니면 표시만?
        int currentCurrency = CurrencyManager.Instance.Coin.Value;
        if (RentManager.Instance.Rent.IsRentDay)
        {
            currentCurrency -= RentManager.Instance.Rent.CurrentRentCost;
        }
        _currentCurrencyTextUI.text = currentCurrency.ToString("N0");

        int totalSales = SalesManager.Instance.Sales.TotalSales;
        _totalSalesTextUI.text = totalSales.ToString("N0");
        _totalDay.text = $"{PhaseManager.Instance.Day} 일";

        _nextRentDay.text = $"{RentManager.Instance.Rent.RentPeriod-RentManager.Instance.Rent.RentDayCounter} 일";
        _nextRentCost.text = RentManager.Instance.Rent.CurrentRentCost.ToString("N0");


        gameObject.SetActive(true);
    }
}
