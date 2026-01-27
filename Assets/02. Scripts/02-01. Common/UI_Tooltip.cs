using DG.Tweening;
using Mirror.BouncyCastle.Asn1.X509;
using UnityEngine;

public enum ETooltipPanel
{
    CommonPreparing,
    CommonServing,
    MarketPreparing,
    MarketServing,
    PracticePreparing
}

public class UI_Tooltip : MonoBehaviour
{
    [SerializeField]
    private RectTransform _preparingCommonPanel;
    [SerializeField]
    private RectTransform _servingCommonPanel;
    [SerializeField]
    private RectTransform _preparingMarketPanel;
    [SerializeField]
    private RectTransform _servingMarketPanel;
    [SerializeField]
    private RectTransform _preparingPracticePanel;


    private RectTransform _currentPanel;
    private RectTransform _nextPanel;


    [SerializeField]
    private float duration = 0.4f;
    [SerializeField]
    private float _hideOffsetX = 350f;

    private float _showOffsetX;
    private Vector2 _targetPos;
    private Vector2 _hidePos;
    

    private void Start()
    {
        _currentPanel = _preparingCommonPanel;
        _showOffsetX = _currentPanel.GetComponent<RectTransform>().anchoredPosition.x;
        _targetPos = new Vector2(_showOffsetX, 0);
        _hidePos = new Vector2(-_hideOffsetX, 0);
    }

    public void ShowNextTooltip(ETooltipPanel tooltipType)
    {
        switch (tooltipType)
        {
            case ETooltipPanel.CommonPreparing:
                _nextPanel = _preparingCommonPanel;
                break;
            case ETooltipPanel.CommonServing:
                _nextPanel = _servingCommonPanel;
                break;
            case ETooltipPanel.MarketPreparing:
                _nextPanel = _preparingMarketPanel;
                break;
            case ETooltipPanel.MarketServing:
                _nextPanel = _servingMarketPanel;
                break;
            case ETooltipPanel.PracticePreparing:
                _nextPanel = _preparingPracticePanel;
                break;
        }

        if (_nextPanel == _currentPanel)
        {
            return;
        }

        // 이전 트윈 Kill
        DOTween.Kill(_currentPanel);
        DOTween.Kill(_nextPanel);

        // CanvasGroup 준비
        CanvasGroup nextGroup = _nextPanel.GetComponent<CanvasGroup>();
        if (nextGroup == null) nextGroup = _nextPanel.gameObject.AddComponent<CanvasGroup>();
        nextGroup.alpha = 0;
        _nextPanel.anchoredPosition = _hidePos;
        _nextPanel.gameObject.SetActive(true);

        CanvasGroup currentGroup = _currentPanel.GetComponent<CanvasGroup>();
        if (currentGroup == null) currentGroup = _currentPanel.gameObject.AddComponent<CanvasGroup>();

        // 전환 시퀀스
        Sequence seq = DOTween.Sequence();

        seq.Join(_currentPanel.DOAnchorPos(_hidePos, duration).SetTarget(_currentPanel))
           .Join(currentGroup.DOFade(0, duration).SetTarget(_currentPanel));

        seq.Join(_nextPanel.DOAnchorPos(_targetPos, duration).SetTarget(_nextPanel))
           .Join(nextGroup.DOFade(1, duration).SetTarget(_nextPanel));

        // ★ 애니메이션 시작 전에 바로 교체
        _currentPanel = _nextPanel;
    }
}
