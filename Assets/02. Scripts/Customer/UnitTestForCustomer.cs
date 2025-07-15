using UnityEngine;

public class UnitTestForCustomer : MonoBehaviour
{
    public void ServiceStart()
    {
        PhaseManager.Instance.TransitionPhase(EPhaseType.ServingPhase);
    }
    public void NextDay()
    {
        PhaseManager.Instance.TransitionPhase(EPhaseType.PreparingPhase);
    }
    public void TestRegisterOrder()
    {
        CustomerManager.Instance.RegisterOrder();
    }
    public void StashHall()
    {
        CustomerManager.Instance.OrderHandler.PotionOrderMap.Clear();
    }
}
