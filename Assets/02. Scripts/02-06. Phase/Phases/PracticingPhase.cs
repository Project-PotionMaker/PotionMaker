using Mirror;
using UnityEngine;

public class PracticingPhase : BasePhase
{
    public PracticingPhase()
    {
        _phaseType = EPhaseType.PracticingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (NetworkServer.active == true)
        {
            CustomerManager.Instance.InviteCustomer(deltaTime); // 손님 초대
        }
    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
