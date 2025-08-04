using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Storage : MonoBehaviour
{
    [SerializeField]
    private Storage _storage;

    [SerializeField]
    private Slider ProgressSlider;
    [SerializeField]
    private TextMeshProUGUI _nameTextUI;
    [SerializeField]
    private TextMeshProUGUI _PriceTextUI;

    [SerializeField]
    private GameObject _interactPanel;
    [SerializeField]
    private GameObject _sliderPanel;

    private void Start()
    {
        _storage.OnDataChanged += Refresh;
        PhaseManager.Instance.OnPhaseChanged += ChangeState;
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

        IngredientData ingredientData = DataTable.Instance.GetIngredientData(_storage.IngredientTID);
        _nameTextUI.text = ingredientData.Name;
        _PriceTextUI.text = ingredientData.Price.ToString();
    }

    public void Refresh()
    {
    }

    private void OnDisable()
    {
        PhaseManager.Instance.OnPhaseChanged -= ChangeState;
    }
}
