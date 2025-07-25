using UnityEngine;

public class LeavingState : MovingState
{
    // 목적지 변수

    public LeavingState(Customer owner) : base(owner)
    {
        StateType = ECustomerStateType.Leaving;
    }
    public override void EnterState()
    {
        // 최종 목적지 설정하고
        // agent를 키든 끄든
        // obstalce 키든 끄든
        // setDestination

        base.EnterState();
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);


        //  x조건이 맞는지

        // y 조건이 맞는지


    }
    public override void ExitState()
    {
        base.ExitState();

        // agent를 키든 끄든
        // obstalce 키든 끄든
        // 뭔가해든
    }
}
