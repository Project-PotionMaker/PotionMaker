using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Furniture : MonoBehaviour
{
    [SerializeField]
    private Furniture _furniture;

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
        _furniture.OnDataChanged += Refresh;
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
        }

        if(_furniture.Data.SpecialStructureType == ESpecialStructureType.Casher)
        {
            _nameTextUI.text = "상점";
        }
        else
        {
            _interactPanel.SetActive(false);
        }
    }

    public void Refresh()
    {
    }

    private void OnDisable()
    {
        PhaseManager.Instance.OnPhaseChanged -= ChangeState;
    }
}
