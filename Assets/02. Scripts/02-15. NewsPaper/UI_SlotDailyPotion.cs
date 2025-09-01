using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_SlotDailyPotion : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private Image _imagePotionIcon;
    [SerializeField]
    private TextMeshProUGUI _textPotionName;
    [SerializeField]
    private TextMeshProUGUI _textPotionArticle;

    public void RefreshSlot(PotionData potionData)
    {
        _imagePotionIcon.sprite = ImageManager.Instance.GetImage<PotionData>(potionData.TID);
        _textPotionName.text = potionData.Name;
        _textPotionArticle.text = potionData.NewsText;
    }
}
