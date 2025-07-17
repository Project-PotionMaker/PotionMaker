using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UI_Phase : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _phaseText;
    [SerializeField]
    private TextMeshProUGUI _dayText;

    private void Start()
    {
        PhaseManager.Instance.OnDayPassed += UpdateDayText;
        PhaseManager.Instance.OnPhaseChanged += UpdatePhaseText;
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
}
