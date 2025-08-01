using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SalesVolumeSlot : MonoBehaviour
{
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private TextMeshProUGUI _salesVolumeTextUI;
    public void Refresh(int potionTID, bool isTotal)
    {
        // Todo: 포션 이미지
        if (isTotal)
        {
            _salesVolumeTextUI.text = SalesManager.Instance.Sales.TotalSalesVolumeDict[potionTID].ToString("N0");
        }
        else
        {
            _salesVolumeTextUI.text = SalesManager.Instance.Sales.DailySalesVolumeDict[potionTID].ToString("N0");
        }
    }
}
