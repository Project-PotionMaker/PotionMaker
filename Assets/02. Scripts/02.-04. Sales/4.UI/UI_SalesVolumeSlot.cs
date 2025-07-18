using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SalesVolumeSlot : MonoBehaviour
{
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private TextMeshProUGUI _salesVolumeTextUI;
    public void Refresh(EPotionType potionType)
    {
        // Todo: 포션 이미지
        _salesVolumeTextUI.text = SalesManager.Instance.Sales.DailySalesVolumeDict[potionType].ToString("N0"); 
    }
}
