using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UI_GameOverVolumeSlot : MonoBehaviour
{
    private const string ASSET_PREFIX = "Image_Potion_";

    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private TextMeshProUGUI _potionNameTextUI;
    [SerializeField]
    private TextMeshProUGUI _salesVolumeTextUI;

    [SerializeField]
    private Sprite _placeholderSprite;

    public void InitializeGameOverVolumeSlot(int potionTID, bool isTotal)
    {
        gameObject.SetActive(true);
        _potionNameTextUI.text = DataTable.Instance.GetPotionData(potionTID).Name;
        if (isTotal)
        {
            //_salesVolumeTextUI.text = SalesManager.Instance.Sales.TotalSalesVolumeDict[potionTID].ToString("N0");
        }
        _potionImage.sprite = _placeholderSprite;
        LoadSpriteAsync(potionTID).SafeFireAndForget();
    }

    private async Task LoadSpriteAsync(int potionTID)
    {
        Sprite loadedSprite = await AssetManager.Instance.LoadAsset<Sprite>($"{ASSET_PREFIX}{potionTID}");

        if (this != null && loadedSprite != null)
        {
            _potionImage.sprite = loadedSprite;
        }
    }
}