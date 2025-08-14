using System.Collections.Generic;
using UnityEngine;

public class UI_NewsPaper : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private List<UI_SlotDailyPotion> _slotDailyPotionList = new();

    public void Start()
    {
        PhaseManager.Instance.DailyPotionPicker.OnPickCompleted += Refresh;
        gameObject.SetActive(false);
    }

    private void Refresh(List<PotionData> dailyPotionDataList)
    {
        int dailyPotionListSize = dailyPotionDataList.Count;
        for (int i = 0; i < _slotDailyPotionList.Count; i++)
        {
            if (i < dailyPotionListSize)
            {
                _slotDailyPotionList[i].gameObject.SetActive(true);
                _slotDailyPotionList[i].RefreshSlot(dailyPotionDataList[i]);
            }
            else
            {
                _slotDailyPotionList[i].gameObject.SetActive(false);
            }
        }
        OpenNewsPaperPopup();
    }

    public void OpenNewsPaperPopup()
    {
        gameObject.SetActive(true);
    }

    public void CloseNewsPaperPopup()
    {
        gameObject.SetActive(false);
    }
}
