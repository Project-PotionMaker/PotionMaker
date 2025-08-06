using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CustomScrollBar : MonoBehaviour
{
    [Header("스크롤바")]
    [SerializeField]
    public Scrollbar targetScrollbar;        // 투명하게 처리할 Scrollbar
    public float fadeDelay = 1f;             // 스크롤 후 투명해지기 전 대기 시간
    public float fadeDuration = 1f;          // 천천히 투명해지는 시간
    private float lastScrollValue;
    [SerializeField]
    private ScrollRect scrollRect;
    private Tween fadeTween;
    private Image[] scrollbarImages;
    private void Start()
    {
        scrollbarImages = targetScrollbar.GetComponentsInChildren<Image>(true);
        SetAlpha(0f);
        lastScrollValue = scrollRect.verticalNormalizedPosition;
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }
    private void OnScrollChanged(Vector2 pos)
    {
        float currentValue = scrollRect.verticalNormalizedPosition;
        if (Mathf.Approximately(currentValue, lastScrollValue))
            return;

        lastScrollValue = currentValue;
        ShowScrollbar();
    }

    private void ShowScrollbar()
    {
        // 즉시 보이기
        fadeTween?.Kill();
        fadeTween = null;

        foreach (var img in scrollbarImages)
        {
            img.DOKill(); // ⬅ 기존 페이드 트윈 중단!

            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }

        // 일정 시간 뒤 부드럽게 사라지기
        fadeTween = DOVirtual.DelayedCall(fadeDelay, () =>
        {
            foreach (var img in scrollbarImages)
            {
                img.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad);
            }
        });
    }

    private void SetAlpha(float a)
    {
        foreach (var img in scrollbarImages)
        {
            Color c = img.color;
            c.a = a;
            img.color = c;
        }
    }
}
