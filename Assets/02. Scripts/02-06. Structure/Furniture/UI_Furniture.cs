using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Furniture : MonoBehaviour
{
    [SerializeField]
    private Furniture _furniture;
    [SerializeField]
    private RefundSystem _refundSystem;
    [SerializeField]
    private Slider ProgressSlider;
    [SerializeField]
    private Slider _refundSlider;
    [SerializeField]
    private TextMeshProUGUI _nameTextUI;
    [SerializeField]
    private GameObject _interactPanel;
    [SerializeField]
    private GameObject _sliderPanel;

    private void Start()
    {
        _furniture.OnDataChanged += Refresh;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PreparingPhase].OnPhaseEntered += ChangeState;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += ChangeState;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseEntered += ChangeState;
        ChangeState();
    }

    public void ChangeState()
    {
        _nameTextUI.text = _furniture.Data.Name;
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            _interactPanel.SetActive(true);
            _sliderPanel.SetActive(false);
        }
        else
        {
            _interactPanel.SetActive(false);
        }

    }

    public void Refresh()
    {
        _nameTextUI.text = _furniture.Data.Name;
        _refundSlider.gameObject.SetActive(_furniture.RefundProgress > 0);
        _refundSlider.value = _furniture.RefundProgress;
    }
}
