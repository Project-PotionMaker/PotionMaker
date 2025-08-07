using System;
using UnityEngine;

public class PreparingPhase : BasePhase
{
    public PreparingPhase()
    {
        _phaseType = EPhaseType.PreparingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
        PhaseManager.Instance.DeathCount = PhaseManager.Instance.MaxDeathCount;
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
