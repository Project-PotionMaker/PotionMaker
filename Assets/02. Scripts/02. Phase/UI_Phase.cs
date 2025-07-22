using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UI_Phase : MonoBehaviour
{
    [Foldout("UIs")]
    [SerializeField]
    private TextMeshProUGUI _phaseText;
    [SerializeField]
    private TextMeshProUGUI _dayText;
    [SerializeField]
    private Slider _serviceTimer;
    [SerializeField]
    private GameObject _todaySummaryPanel;
    [SerializeField]
    private TextMeshProUGUI _todaySummaryText;
    [SerializeField]
    private Button _nextDayButton;

    private void Start()
    {
        _serviceTimer.maxValue = 1f;
        PhaseManager.Instance.OnDayPassed += UpdateDayText;
        PhaseManager.Instance.OnPhaseChanged += UpdatePhaseText;
        ServingPhase servingPhase = (ServingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase];
        servingPhase.OnTimerRunning += UpdateServiceTimer;
        servingPhase.OnPhaseEntered += ShowTimer; // 타이머 시작 시 업데이트
        servingPhase.OnPhaseExited += HideTimer;
        EndingPhase endingPhase = (EndingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.EndingPhase];
        endingPhase.OnPhaseEntered += ShowSummary; // 영업 종료 시 요약 패널 표시
        endingPhase.OnPhaseExited += HideSummary; // 영업 종료 후 요약 패널 숨김
    }

    private void UpdateDayText()
    {
        if (_dayText != null)
        {
            _dayText.text = "Day: " + PhaseManager.Instance.Day;
        }
    }

    private void UpdatePhaseText()
    {
        if (_phaseText != null)
        {
            EPhaseType phaseType = PhaseManager.Instance.CurrentPhase.PhaseType;
            if (phaseType == EPhaseType.PreparingPhase)
            {
                _phaseText.text = "Preparing";
            }
            else if (phaseType == EPhaseType.ServingPhase)
            {
                _phaseText.text = "Service Time";
            }
            else if (phaseType == EPhaseType.EndingPhase)
            {
                _phaseText.text = "Finish";
            }
        }
    }
    private void UpdateServiceTimer(float time)
    {
        _serviceTimer.value = time;
    }

    private void ShowTimer()
    {
        _serviceTimer.gameObject.SetActive(true);
    }
    private void HideTimer()
    {
        _serviceTimer.gameObject.SetActive(false);
    }

    private void ShowSummary()
    {
        _todaySummaryPanel.SetActive(true);
        _todaySummaryText.text = $"Gold : {SalesManager.Instance.Sales.DailySales}";
    }
    private void HideSummary()
    {
        _todaySummaryPanel.SetActive(false);
    }

}
