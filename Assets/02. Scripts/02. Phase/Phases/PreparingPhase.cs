using System;
using UnityEngine;
using Mirror;

public class PreparingPhase : BasePhase
{
    public PreparingPhase()
    {
        _phaseType = EPhaseType.PreparingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
        if(NetworkServer.active)
        {
            PhaseManager.Instance.ResetDeathCount();
        }
        AudioManager.Instance.PlaySFX(EPhaseAudioType.EnterPreparingPhase);
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
