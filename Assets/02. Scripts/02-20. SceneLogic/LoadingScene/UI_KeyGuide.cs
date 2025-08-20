using DG.Tweening;
using UnityEngine;

public class UI_KeyGuide : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _fadeDuration = 0.5f;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        FadeInGuide();
    }

    private void FadeInGuide()
    {
        _canvasGroup.DOFade(1f, _fadeDuration);
    }
}
