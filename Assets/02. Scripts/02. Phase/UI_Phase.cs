using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using DG.Tweening;

public class UI_Phase : MonoBehaviour
{
    [Foldout("UIs")]
    [SerializeField]
    private TextMeshProUGUI _currencyText;
    [SerializeField]
    private TextMeshProUGUI _dayText;
    [SerializeField]
    private Slider _serviceTimer;
    [SerializeField]
    private GameObject _todaySummaryPanel;
    [SerializeField]
    private TextMeshProUGUI _todaySummaryText;
    [SerializeField]
    private GameObject _startDayPanel;
    [SerializeField]
    private TextMeshProUGUI _startDayText;
    [SerializeField]
    private GameObject[] _isVoted;

    private const float HIDE_OFFSET = 400f;
    private const float DURATION = 1f;

    private void Start()
    {
        _serviceTimer.maxValue = 1f;
        PhaseManager.Instance.OnDayPassed += UpdateDayText;
        PhaseManager.Instance.OnPhaseChanged += UpdatePhaseText;

        PreparingPhase preparingPhase = (PreparingPhase) PhaseManager.Instance.PhaseDictionary[EPhaseType.PreparingPhase];
        preparingPhase.OnPhaseEntered += ChangeTextStartDay;
        PracticingPhase practicingPhase = (PracticingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase];
        practicingPhase.OnPhaseEntered += ChangeTextPracticeEnd;
        ServingPhase servingPhase = (ServingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase];
        servingPhase.OnTimerRunning += UpdateServiceTimer;
        servingPhase.OnPhaseEntered += ShowTimer; // 타이머 시작 시 업데이트
        servingPhase.OnPhaseExited += HideTimer;
        servingPhase.OnPhaseEntered += HideStartDay; // 준비 단계가 끝나면 시작 패널 숨김
        HideTimer();
        EndingPhase endingPhase = (EndingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.EndingPhase];
        endingPhase.OnPhaseEntered += ShowSummary; // 영업 종료 시 요약 패널 표시
        endingPhase.OnPhaseExited += HideSummary; // 영업 종료 후 요약 패널 숨김
        endingPhase.OnPhaseExited += ShowStartDay; // 준비 단계가 시작되면 시작 패널 표시
        HideSummary();
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
        if (_currencyText != null)
        {
            EPhaseType phaseType = PhaseManager.Instance.CurrentPhase.PhaseType;
            if (phaseType == EPhaseType.PreparingPhase)
            {
                _currencyText.text = "Preparing";
            }
            else if (phaseType == EPhaseType.ServingPhase)
            {
                _currencyText.text = "Service Time";
            }
            else if (phaseType == EPhaseType.EndingPhase)
            {
                _currencyText.text = "Finish";
            }
            else if (phaseType == EPhaseType.PracticingPhase)
            {
                _currencyText.text = "Practicing";
            }
        }
    }
    private void UpdateServiceTimer()
    {
        _serviceTimer.value = ((ServingPhase) PhaseManager.Instance.CurrentPhase).CurrentTimeRate;
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

    private void HideStartDay()
    {
        Debug.Log("Hide Start Day Panel");
        _startDayPanel.transform.DOLocalMoveY(HIDE_OFFSET, DURATION).SetRelative().SetEase(Ease.OutSine);
    }
    private void ShowStartDay()
    {
        _startDayPanel.transform.DOLocalMoveY(-HIDE_OFFSET, DURATION).SetRelative().SetEase(Ease.OutSine);
    }
    private void ChangeTextStartDay()
    {
        _startDayText.text = "영업 시작";
    }
    private void ChangeTextPracticeEnd()
    {
        _startDayText.text = "연습 종료";
    }

}
