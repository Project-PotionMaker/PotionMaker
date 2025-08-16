using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_OutputItem : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _fadeDuration;

    [SerializeField]
    private List<Image> _availableMachineImageList = new();

    [SerializeField]
    private List<Image> _usedIngredientImageList = new();

    private void Awake()
    {
        GetComponent<OutputItem>().OnOutputTIDUpdated += Refresh;
        GetComponent<OutputItem>().OnItemHighlighted += HandleUIFade;
        _canvasGroup.alpha = 0f;
    }

    private void Refresh(List<int> availableMachineTIDList, List<int> usedIngredientTIDList)
    {
        RefreshAvailableMachineImages(availableMachineTIDList);
        RefreshUsedIngredientImages(usedIngredientTIDList);
        _canvasGroup.alpha = 0f;
    }

    private void RefreshAvailableMachineImages(List<int> availableMachineTIDList)
    {
        for (int i = 0; i < _availableMachineImageList.Count; i++)
        {
            if (availableMachineTIDList.Count <= i)
            {
                break;
            }

            if (availableMachineTIDList[i] == 0)
            {
                _availableMachineImageList[i].transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _availableMachineImageList[i].transform.parent.gameObject.SetActive(true);
                _availableMachineImageList[i].sprite = ImageManager.Instance.GetImage<MachineData>(availableMachineTIDList[i]);
            }
        }
    }

    private void RefreshUsedIngredientImages(List<int> usedIngredientTIDList)
    {
        for (int i = 0; i < _usedIngredientImageList.Count; i++)
        {
            if (usedIngredientTIDList.Count <= i)
            {
                break;
            }

            if (usedIngredientTIDList[i] == 0)
            {
                _usedIngredientImageList[i].transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _usedIngredientImageList[i].transform.parent.gameObject.SetActive(true);
                _usedIngredientImageList[i].sprite = ImageManager.Instance.GetImage<IngredientData>(usedIngredientTIDList[i]);
            }
        }
    }

    private void HandleUIFade(bool isActive)
    {
        if (isActive)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        _canvasGroup.DOFade(1f, _fadeDuration);
    }

    private void FadeOut()
    {
        _canvasGroup.DOFade(0f, _fadeDuration);
    }
}
