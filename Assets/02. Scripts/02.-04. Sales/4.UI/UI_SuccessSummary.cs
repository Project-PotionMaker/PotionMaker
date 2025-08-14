using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SuccessSummary : MonoBehaviour
{
    private const string POSITIVE_COLOR = "#65BC04";
    private const string NEGATIVE_COLOR = "#CB0000";

    private LightColorByPhase _lightColorByPhase;

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
    private UI_VoteSystem _voteSystem;

    private void Start()
    {
        gameObject.SetActive(false);
        _voteSystem = GetComponent<UI_VoteSystem>();
        Light light = FindAnyObjectByType<Light>();
        _lightColorByPhase = light.GetComponent<LightColorByPhase>();
        _voteSystem.enabled = false; 
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

        VoteManager.Instance.OnVoteDone += NextPhase;
        VoteManager.Instance.OnVoteDone += StopVoting;
        _voteSystem.enabled = true;
    }

    private void NextPhase()
    {
        if(NetworkServer.active == false)
        {
            return;
        }

        HidePanel();
        _lightColorByPhase.DayChangeLight();
    }
    private void StopVoting()
    {
        VoteManager.Instance.OnVoteDone -= NextPhase;
        VoteManager.Instance.OnVoteDone -= StopVoting;
        _voteSystem.enabled = false;
    }

    private void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
