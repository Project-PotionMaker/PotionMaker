using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_NewsPaper : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private List<UI_SlotDailyPotion> _slotDailyPotionList = new();

    public void Start()
    {
        PhaseManager.Instance.OnPickCompleted += Refresh;
        gameObject.SetActive(false);
    }

    private void Refresh(List<PotionData> dailyPotionTIDList)
    {
        GameSceneUIManager.Instance?.OpenNewspaperPopup();
        AudioManager.Instance.PlaySFX(EPhaseAudioType.EnterPreparingPhase);

        Debug.Log("UI_NewsPaper의 Refresh");
        int dailyPotionListSize = dailyPotionTIDList.Count;
        for (int i = 0; i < _slotDailyPotionList.Count; i++)
        {
            if (i < dailyPotionListSize)
            {
                _slotDailyPotionList[i].gameObject.SetActive(true);
                _slotDailyPotionList[i].RefreshSlot(dailyPotionTIDList[i]);
                //_slotDailyPotionList[i].RefreshSlot(DataTable.Instance.GetPotionData(dailyPotionTIDList[i]));
                _slotDailyPotionList[i].transform.DOScale(Vector3.one, 0.5f).From(Vector3.one * 1.2f)
                    .SetEase(Ease.InOutQuad)
                    .SetRelative(true);
            }
            else
            {
                _slotDailyPotionList[i].gameObject.SetActive(false);
            }
        }
    }
}
