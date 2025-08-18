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
        ChangeState();
    }

    public void ChangeState()
    {
        _interactPanel.SetActive(true);

        IngredientData ingredientData = DataTable.Instance.GetIngredientData(_storage.IngredientTID);
        if(ingredientData == null)
        {
            return;
        }
        _nameTextUI.text = ingredientData.Name;
        _PriceTextUI.text = ingredientData.Price.ToString();
    }

    public void Refresh()
    {
        IngredientData ingredientData = DataTable.Instance.GetIngredientData(_storage.IngredientTID);
        _nameTextUI.text = ingredientData.Name;
        _PriceTextUI.text = ingredientData.Price.ToString();
    }

    private void OnDisable()
    {
        PhaseManager.Instance.OnPhaseChanged -= ChangeState;
    }
}
