using System.Collections.Generic;
using UnityEngine;

public class UI_NewsPaper : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private List<UI_SlotDailyPotion> _slotDailyPotionList = new();

    private List<PotionData> _dailyPotionDataList = new();

    public void Start()
    {
        PhaseManager.Instance.OnDayPassed += Refresh;
    }

    public void Refresh()
    {
        //PhaseManager에서 DailyPotionPicker를 들고있고, 날짜가 바뀔때마다
        //DailyPotionPicker.PickDailyPotion을 통해 DailyPotionList를 갱신해주면 됩니다.

        // _dailyPotionDataList = PhaseManager.Instance.DailyPotionList;
        //int dailyPotionListSize = _dailyPotionDataList.Count;
        //for (int i = 1; i <= _slotDailyPotionList.Count; i++)
        //{
        //    if (i <= dailyPotionListSize)
        //    {
        //        _slotDailyPotionList[i - 1].gameObject.SetActive(true);
        //        _slotDailyPotionList[i - 1].RefreshSlot(_dailyPotionDataList[i - 1]);
        //    }
        //    else
        //    {
        //        _slotDailyPotionList[i - 1].gameObject.SetActive(false);
        //    }
        //}
    }

    public void OpenNewsPaperPopup()
    {
        gameObject.SetActive(true);
    }

    public void CloseNewsPaperPopup()
    {
        gameObject.SetActive(true);
    }

}
