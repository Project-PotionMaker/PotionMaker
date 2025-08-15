using System.Collections.Generic;
using UnityEngine;

public class UI_NewsPaper : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private List<UI_SlotDailyPotion> _slotDailyPotionList = new();

    public void Start()
    {
        PhaseManager.Instance.OnPickCompleted += Refresh;
        Debug.Log("OnPickCompleted에 UI_NewsPaper의 Refresh 등록");
        gameObject.SetActive(false);
    }

    private void Refresh(List<int> dailyPotionTIDList)
    {
        Debug.Log("UI_NewsPaper의 Refresh");
        int dailyPotionListSize = dailyPotionTIDList.Count;
        for (int i = 0; i < _slotDailyPotionList.Count; i++)
        {
            if (i < dailyPotionListSize)
            {
                _slotDailyPotionList[i].gameObject.SetActive(true);
                _slotDailyPotionList[i].RefreshSlot(DataTable.Instance.GetPotionData(dailyPotionTIDList[i]));
            }
            else
            {
                _slotDailyPotionList[i].gameObject.SetActive(false);
            }
        }
        GameSceneUIManager.Instance?.OpenNewspaperPopup();
    }
}
