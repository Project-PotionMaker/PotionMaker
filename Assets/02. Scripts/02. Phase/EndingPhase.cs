using UnityEngine;
using System;

public class EndingPhase : BasePhase
{
    public EndingPhase()
    {
        _phaseType = EPhaseType.EndingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
