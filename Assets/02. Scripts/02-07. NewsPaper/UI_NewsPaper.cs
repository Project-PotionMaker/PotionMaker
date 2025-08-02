using System.Collections.Generic;
using UnityEngine;

public class UI_NewsPaper : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private List<UI_SlotDailyPotion> _slotDailyPotionList = new();

    public void Start()
    {
        // PhaseManager.Instance.DailyPotionPicker.OnPickCompleted += Refresh;
    }

    private void Refresh(List<PotionData> dailyPotionDataList)
    {
        //PhaseManager에서 DailyPotionPicker를 들고있고, 날짜가 바뀔때마다
        //DailyPotionPicker.PickDailyPotion을 통해 DailyPotionList를 갱신해주면 됩니다.
        // 매니저에서는 현재 포션 상점의 티어만 넣어주시면, PickDailyPotion에서 평판까지 고려한
        // 오늘 등장하는 포션데이터 리스트를 던져줄 것입니다.

        int dailyPotionListSize = dailyPotionDataList.Count;
        for (int i = 1; i <= _slotDailyPotionList.Count; i++)
        {
            if (i <= dailyPotionListSize)
            {
                _slotDailyPotionList[i - 1].gameObject.SetActive(true);
                _slotDailyPotionList[i - 1].RefreshSlot(dailyPotionDataList[i - 1]);
            }
            else
            {
                _slotDailyPotionList[i - 1].gameObject.SetActive(false);
            }
        }
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
