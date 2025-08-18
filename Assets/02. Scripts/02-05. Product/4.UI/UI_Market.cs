using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Market : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _coinValueTextUI;
    [SerializeField]
    private Transform _slotContainer;
    private List<UI_ProductSlot> _productSlotList;
    [SerializeField]
    private UI_ProductSlot _productSlotPrefab;

    [SerializeField]
    private UI_DetailPage _detailPage;

    [SerializeField]
    private TextMeshProUGUI _alertTextUI;
    [SerializeField]
    private CanvasGroup _alertPanelGroup;
    private Sequence _alertSeq;


    public void OnMachineButtonClicked() => OnProductTypeButtonClicked(EProductType.Machine);
    public void OnFurnitureButtonClicked() => OnProductTypeButtonClicked(EProductType.Furniture);
    public void OnHouseMovingButtonClicked() => OnProductTypeButtonClicked(EProductType.HouseMoving);

    private void Awake()
    {
        _productSlotList = new List<UI_ProductSlot>();
    }

    private void OnEnable()
    {
        RefreshCoin();
        OnProductTypeButtonClicked(EProductType.Machine);
        //RefreshDetailPage(_productSlotList[0])

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(EPopupAudioType.Market);
        }
    }

    private void Start()
    {
        CurrencyManager.OnDataChanged += RefreshCoin;
        ProductManager.Instance.OnBuyResultReceived += AlertBuyResult;
        gameObject.SetActive(false);
    }

    public void OnProductTypeButtonClicked(EProductType productType)
    {
        bool isDetailPageRefreshed = false;
        int slotIndex = 0;
        foreach(ProductDTO productDTO in ProductManager.Instance.ProductListDict[productType])
        {
            if (!productDTO.IsUnlocked)
            {
                continue;
            }
            if(slotIndex >= _productSlotList.Count)
            {
                UI_ProductSlot newSlot = Instantiate(_productSlotPrefab, _slotContainer);
                newSlot.OnSlotClicked += RefreshDetailPage;
                _productSlotList.Add(newSlot);
            }
            _productSlotList[slotIndex].Refresh(productDTO);
            ++slotIndex;

            if (!isDetailPageRefreshed)
            {
                isDetailPageRefreshed = true;
                _detailPage.Refresh(productDTO);
            }
        }

        for (int deleteIndex = _productSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_ProductSlot deleteSlot = _productSlotList[deleteIndex];
            deleteSlot.OnSlotClicked -= RefreshDetailPage;
            _productSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }
    }

    public void RefreshDetailPage(ProductDTO productDTO)
    {
        _detailPage.Refresh(productDTO);
    }

    public void RefreshCoin()
    {
        _coinValueTextUI.text = CurrencyManager.Instance.Coin.Value.ToString("N0");
    }

    public void AlertBuyResult(ResultMessage resultMessage)
    {
        if (!resultMessage.Result)
        {
            _alertTextUI.text = resultMessage.Message;
            const float fadeDur = 0.35f; // 패널 페이드 인/아웃 시간
            const float blinkDur = 0.3f;  // 한 번 깜빡임의 반 주기(투명 또는 불투명 전환 시간)


            // 진행 중 트윈 정리
            _alertPanelGroup.DOKill();
            _alertTextUI.DOKill();
            _alertSeq?.Kill();

            _alertPanelGroup.alpha = 0f;      // 패널은 보이지 않는 상태에서 시작

            _alertTextUI.alpha = 0f;            // 텍스트는 보이는 상태에서 시작

            _alertSeq = DOTween.Sequence()
                .Append(_alertPanelGroup.DOFade(1f, fadeDur).SetEase(Ease.OutSine))
                .Append(_alertTextUI.DOFade(1f, blinkDur))
                .Append(_alertTextUI.DOFade(0f, blinkDur))
                .Append(_alertTextUI.DOFade(1f, blinkDur))
                .Append(_alertTextUI.DOFade(0f, blinkDur))
                .Append(_alertPanelGroup.DOFade(0f, fadeDur).SetEase(Ease.OutSine));
        }
    }
}
