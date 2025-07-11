using System;
using UnityEngine;

public class PreparingPhase : BasePhase
{
    public event Action OnPreparingPhaseEntered;
    public event Action OnPreparingPhaseExited;

    public PreparingPhase()
    {
        _phaseType = EPhaseType.PreparingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
        OnPreparingPhaseEntered?.Invoke();
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
        OnPreparingPhaseExited?.Invoke();
    }
}
