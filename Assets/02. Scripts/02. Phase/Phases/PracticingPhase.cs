using UnityEngine;

public class PracticingPhase : BasePhase
{
    public PracticingPhase()
    {
        _phaseType = EPhaseType.PracticingPhase;
    }
    public override void EnterPhase()
    {
        CustomerManager.Instance.PreService();
        base.EnterPhase();
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        CustomerManager.Instance.InviteCustomer(deltaTime); // 손님 초대
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
