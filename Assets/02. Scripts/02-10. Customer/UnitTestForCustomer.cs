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
        foreach(var customer in CustomerManager.Instance.OrderHandler.PotionOrderMap[10000])
        {
            CustomerManager.Instance.LineHandler.PutOutCustomer(customer);
        }
    }
    public void TestServePotion()
    {
        CustomerManager.Instance.ServePotion(10000);
    }
}
