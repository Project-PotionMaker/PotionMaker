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
        ((ServingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase]).OnTimerRunning += UpdateServiceTimer;
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
}
