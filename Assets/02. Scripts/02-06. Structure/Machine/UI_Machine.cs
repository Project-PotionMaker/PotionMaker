using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Machine : MonoBehaviour
{
    [SerializeField]
    private Machine _machine;
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
    private GameObject _successPanel;
    [SerializeField]
    private GameObject _sliderPanel;

    private void Start()
    {
        _machine.OnDataChanged += Refresh;
        PhaseManager.Instance.OnPhaseChanged += ChangeState;
        ChangeState();
        Refresh();
    }

    public void ChangeState()
    {
        _nameTextUI.text = _machine.Data.Name;
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            _interactPanel.SetActive(true);
        }
        else
        {
            _interactPanel.SetActive(false);
        }
    }

    public void Refresh()
    {
        _nameTextUI.text = _machine.Data.Name;

        if(_machine.CurrentProgress > 0 && _machine.IsProcessFinished == false)
        {
            ProgressSlider.gameObject.SetActive(true);
        }
        else if(_machine.CurrentProgress == 0 || _machine.IsProcessFinished)
        {
            ProgressSlider.gameObject.SetActive(false);
        }
        
        if (_machine.IsProcessFinished)
        {
            _successPanel.SetActive(true);
        }
        else
        {
            _successPanel.SetActive(false);
        }

        ProgressSlider.value = _machine.CurrentProgress / _machine.Data.MaxProgress;
        _refundSlider.gameObject.SetActive(_machine.RefundProgress > 0);
        _refundSlider.value = _machine.RefundProgress;
    }

    private void OnDisable()
    {
        PhaseManager.Instance.OnPhaseChanged -= ChangeState;
    }
}