using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Machine : MonoBehaviour
{
    [SerializeField]
    private Machine _machine;

    [SerializeField]
    private Slider ProgressSlider;
    [SerializeField]
    private TextMeshProUGUI _nameTextUI;
    [SerializeField]
    private GameObject _interactPanel;
    [SerializeField]
    private GameObject _sliderPanel;

    private void Start()
    {
        _machine.OnDataChanged += Refresh;
        PhaseManager.Instance.OnPhaseChanged += ChangeState;
        ChangeState();
    }

    public void ChangeState()
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            _interactPanel.SetActive(true);
            _sliderPanel.SetActive(false);
        }
        else
        {
            _interactPanel.SetActive(false);
            _sliderPanel.SetActive(true);
        }

        // 테스트
        _interactPanel.SetActive(false);
    }

    public void Refresh()
    {
        _nameTextUI.text = _machine.Data.Name;

        ProgressSlider.value = _machine.CurrentProgress / _machine.Data.MaxProgress;
    }

    private void OnDisable()
    {
        PhaseManager.Instance.OnPhaseChanged -= ChangeState;
    }
}