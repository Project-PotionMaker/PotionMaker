using System;
using UnityEngine;

public class TooltipManager : MonoBehaviourSingleton<TooltipManager>
{
    [SerializeField]
    private UI_Tooltip TooltipUI;

    public void Start()
    {
        PhaseManager.Instance.OnPhaseChanged += ChangeTooltipOnPhase;
    }

    public void ChangeTooltipOnPhase()
    {
        EPhaseType currentPhase = PhaseManager.Instance.CurrentPhase.PhaseType;
        if(currentPhase == EPhaseType.PreparingPhase)
        {
            ShowTooltip(ETooltipPanel.CommonPreparing);
        }
        else if (currentPhase == EPhaseType.ServingPhase)
        {
            ShowTooltip(ETooltipPanel.CommonServing);
        }
    }

    public void ShowTooltip(ETooltipPanel tooltipType)
    {
        TooltipUI.ShowNextTooltip(tooltipType);
    }
}
