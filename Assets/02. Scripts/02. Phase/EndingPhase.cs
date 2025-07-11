using UnityEngine;
using System;

public class EndingPhase : BasePhase
{
    public event Action OnEndingPhaseEntered;
    public event Action OnEndingPhaseExited;

    public EndingPhase()
    {
        _phaseType = EPhaseType.EndingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
        OnEndingPhaseEntered?.Invoke();
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
        OnEndingPhaseExited?.Invoke();
    }
}
