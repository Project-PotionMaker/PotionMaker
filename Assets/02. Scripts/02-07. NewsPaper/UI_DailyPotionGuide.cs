using System.Collections.Generic;
using UnityEngine;

public class UI_DailyPotionGuide : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private Transform _slotContainer;
    private List<UI_DailyPotionGuideSlot> _dailyPotionGuideSlotList;
    [SerializeField]
    private UI_DailyPotionGuideSlot _dailyPotionGuideSlotPrefab;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        PhaseManager.Instance.OnPickCompleted += Refresh;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += Hide;
    }

    private void OnDestroy()
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.OnPickCompleted -= Refresh;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited -= Hide;
        }
    }

    private void Refresh(List<PotionData> dailyPotionList)
    {
        int slotIndex = 0;

        foreach (PotionData potionData in dailyPotionList)
        {
            if (slotIndex >= _dailyPotionGuideSlotList.Count)
            {
                UI_DailyPotionGuideSlot newSlot = Instantiate(_dailyPotionGuideSlotPrefab, _slotContainer);
                _dailyPotionGuideSlotList.Add(newSlot);
            }
            _dailyPotionGuideSlotList[slotIndex].Refresh(potionData);
            ++slotIndex;
        }

        for (int deleteIndex = _dailyPotionGuideSlotList.Count - 1; deleteIndex >= slotIndex; --deleteIndex)
        {
            UI_DailyPotionGuideSlot deleteSlot = _dailyPotionGuideSlotList[deleteIndex];
            _dailyPotionGuideSlotList.RemoveAt(deleteIndex);
            Destroy(deleteSlot.gameObject);
        }

        _canvasGroup.alpha = 1;
    }

    private void Hide()
    {
        _canvasGroup.alpha = 0;
    }
}
