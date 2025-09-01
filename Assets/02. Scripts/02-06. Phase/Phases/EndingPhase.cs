public class EndingPhase : BasePhase
{
    public EndingPhase()
    {
        _phaseType = EPhaseType.EndingPhase;
    }

    public override void EnterPhase()
    {
        base.EnterPhase();
        AudioManager.Instance.PlaySFX(EPhaseAudioType.EnterEndingPhase);
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
