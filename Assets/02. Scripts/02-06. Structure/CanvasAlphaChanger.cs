using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CanvasAlphaChanger : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _thisCanvasGroup;
    [SerializeField]
    private Slider progressBar;

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
        if(progressBar.value < 0.1f)
        {
            _thisCanvasGroup.DOKill();
            _thisCanvasGroup.DOFade(0f, 0.25f);
        }
    }
}
