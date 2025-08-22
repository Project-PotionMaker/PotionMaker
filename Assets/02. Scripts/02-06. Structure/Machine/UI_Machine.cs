using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private List<GameObject> _inputIngredientPanelList;
    [SerializeField]
    private List<Image> _inputIngredientImageList;

    private void Start()
    {
        _machine.OnDataChanged += Refresh;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PreparingPhase].OnPhaseEntered += ChangeState;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += ChangeState;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseEntered += ChangeState;

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

        bool isInProgress = _machine.CurrentProgress > 0 && !_machine.IsProcessFinished;

        ProgressSlider.gameObject.SetActive(isInProgress);
        _successPanel.SetActive(_machine.IsProcessFinished);

        ProgressSlider.value = _machine.CurrentProgress / _machine.Data.MaxProgress;
        _refundSlider.gameObject.SetActive(_machine.RefundProgress > 0);
        _refundSlider.value = _machine.RefundProgress;

        int imageListIndex = 0;
        Sprite sprite;
        foreach(int inputTID in _machine.InputTIDList)
        {
            switch (_machine.InputType)
            {
                case EInputType.Ingredient:
                {
                    sprite = ImageManager.Instance.GetImage<IngredientData>(inputTID);
                    _inputIngredientImageList[imageListIndex].sprite = sprite;
                    _inputIngredientPanelList[imageListIndex++].gameObject.SetActive(true);

                    break;
                }
                case EInputType.Output:
                {
                    foreach(int ingredientTID in DataTable.Instance.GetOutputData(inputTID).IngredientTIDList)
                    {
                        if(ingredientTID == 0)
                        {
                            continue;
                        }
                        sprite = ImageManager.Instance.GetImage<IngredientData>(ingredientTID);
                        _inputIngredientImageList[imageListIndex].sprite = sprite;
                        _inputIngredientPanelList[imageListIndex++].gameObject.SetActive(true);
                    }

                    break;
                }
            }
        }

        for(int inactiveImageIndex = imageListIndex; inactiveImageIndex<_inputIngredientImageList.Count; inactiveImageIndex++)
        {
            Debug.Log(inactiveImageIndex);
            _inputIngredientPanelList[inactiveImageIndex].gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PreparingPhase].OnPhaseEntered -= ChangeState;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered -= ChangeState;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseEntered -= ChangeState;
    }
}