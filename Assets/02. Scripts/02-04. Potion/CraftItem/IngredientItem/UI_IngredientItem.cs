using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UI_IngredientItem : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _fadeDuration;

    [SerializeField]
    private Image _availableMachineImage;

    private void Awake()
    {
        GetComponent<IngredientItem>().OnItemTIDUpdated += Refresh;
        GetComponent<IngredientItem>().OnItemHighlighted += HandleUIFade;
        _canvasGroup.alpha = 0f;
    }

    private void Refresh(int availableMachineTID)
    {
        _availableMachineImage.sprite = ImageManager.Instance.GetImage<MachineData>(availableMachineTID);
        _canvasGroup.alpha = 0f;
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
