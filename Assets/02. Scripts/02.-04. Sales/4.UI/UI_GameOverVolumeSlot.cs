using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOverVolumeSlot : MonoBehaviour
{
    private const string ASSET_PREFIX = "Image_Potion_";

    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private TextMeshProUGUI _potionNameTextUI;
    [SerializeField]
    private TextMeshProUGUI _salesVolumeTextUI;

    public async void Refresh(int potionTID, bool isTotal)
    {
        gameObject.SetActive(false);
        // Todo: 포션 이미지


        // total 새로 만들어야 될 것 같던데 추후에 없앨듯
        if (isTotal)
        {
            _salesVolumeTextUI.text = SalesManager.Instance.Sales.TotalSalesVolumeDict[potionTID].ToString("N0");
        }
        else
        {
            _potionImage.sprite = await AssetManager.Instance.LoadAsset<Sprite>($"{ASSET_PREFIX}{potionTID}");
            _potionNameTextUI.text = DataTable.Instance.GetPotionData(potionTID).Name;
            int salesVolume = SalesManager.Instance.Sales.DailySalesVolumeDict[potionTID];
            _salesVolumeTextUI.text = salesVolume.ToString("N0");
        }
        gameObject.SetActive(true);
    }
}
