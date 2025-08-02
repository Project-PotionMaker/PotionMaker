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

    public void Refresh(int potionTID, bool isTotal)
    {
        // Todo: 포션 이미지
        if (isTotal)
        {
            _salesVolumeTextUI.text = SalesManager.Instance.Sales.TotalSalesVolumeDict[potionTID].ToString("N0");
        }
        else
        {
            int salesVolume = SalesManager.Instance.Sales.DailySalesVolumeDict[potionTID];
            _salesVolumeTextUI.text = salesVolume.ToString("N0");
            _salesAmountTextUI.text = (salesVolume * DataTable.Instance.GetPotionData(potionTID).Price).ToString("N0");
        }
    }
}
