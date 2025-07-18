using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Phase : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _phaseText;
    [SerializeField]
    private TextMeshProUGUI _dayText;
    [SerializeField]
    private Slider _serviceTimer;

    private void Start()
    {
        _serviceTimer.maxValue = 1f;
        PhaseManager.Instance.OnDayPassed += UpdateDayText;
        PhaseManager.Instance.OnPhaseChanged += UpdatePhaseText;
        ServingPhase servingPhase = (ServingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase];
        servingPhase.OnTimerRunning += UpdateServiceTimer;
        servingPhase.OnPhaseEntered += ShowTimer; // 타이머 시작 시 업데이트
        servingPhase.OnPhaseExited += HideTimer;
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
            if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
            {
                _phaseText.text = "Preparing";
            }
            else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
            {
                _phaseText.text = "Service Time";
            }
            else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.EndingPhase)
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

}
