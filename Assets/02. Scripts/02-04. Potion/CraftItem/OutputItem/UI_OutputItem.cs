using DG.Tweening;
using System;
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
        RefreshImages(_availableMachineImageList, availableMachineTIDList, tid => ImageManager.Instance.GetImage<MachineData>(tid));
    }

    private void RefreshUsedIngredientImages(List<int> usedIngredientTIDList)
    {
        RefreshImages(_usedIngredientImageList, usedIngredientTIDList, tid => ImageManager.Instance.GetImage<IngredientData>(tid));
    }

    private void RefreshImages(IReadOnlyList<Image> imageList, IReadOnlyList<int> tidList, Func<int, Sprite> getSprite)
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            bool shouldShow = i < tidList.Count && tidList[i] != 0;
            GameObject parentObject = imageList[i].transform.parent.gameObject;
            parentObject.SetActive(shouldShow);

            if (shouldShow)
            {
                imageList[i].sprite = getSprite(tidList[i]);
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
