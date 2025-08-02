using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SalesVolumeSlot : MonoBehaviour
{
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private TextMeshProUGUI _salesVolumeTextUI;
    [SerializeField]
    private TextMeshProUGUI _salesAmountTextUI;
    public void Refresh(EPotionType potionType, bool isTotal)
    {
        // Todo: 포션 이미지
        if (isTotal)
        {
            _salesVolumeTextUI.text = SalesManager.Instance.Sales.TotalSalesVolumeDict[potionType].ToString("N0");
        }
        else
        {
            int salesVolume = SalesManager.Instance.Sales.DailySalesVolumeDict[potionType];
            _salesVolumeTextUI.text = salesVolume.ToString("N0");
            // enum이 아닌 TID 받는걸로 바뀜 - 데이터테이블에서 가격 받아와서 채우기
            // _salesAmountTextUI.text = salesVolume * DataTable.Instance.GetPotionData()
        }
    }
}
