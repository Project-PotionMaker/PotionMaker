using UnityEngine;
using DG.Tweening;

public class CanvasAlphaChanger : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _thisCanvasGroup;

    private void Awake()
    {
        _thisCanvasGroup.alpha = 0f;
    }

    public void ShowCanvas()
    {
        _thisCanvasGroup.DOKill();
        _thisCanvasGroup.DOFade(1f, 0.25f);
    }

    public void HideCanvas()
    {
        _thisCanvasGroup.DOKill();
        _thisCanvasGroup.DOFade(0f, 0.25f);
    }
}
