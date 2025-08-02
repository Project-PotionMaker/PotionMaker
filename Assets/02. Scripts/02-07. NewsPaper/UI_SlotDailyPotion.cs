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

    private void Start()
    {
        
    }

    public void RefreshSlot(PotionData potionData)
    {
        // 포션에 대응하는 이미지를 어디서, 어떻게 관리해줄 것인가?
        //_imagePotionIcon.sprite = 포션에 대응하는 이미지;
        _textPotionName.text = potionData.Name;
        _textPotionArticle.text = $"{potionData.Name}이 등장합니다.";
    }
}
