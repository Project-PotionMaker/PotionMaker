using UnityEngine;

public class UnitTestForCustomer : MonoBehaviour
{
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        {
            //if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
               // ServiceStart();
        }
    }
    public void ServiceStart()
    {
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.ServingPhase);
    }
    public void PracticeStart()
    {
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.PracticingPhase);
    }
    public void PracticeEnd()
    {
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.PreparingPhase);
    }
    public void NextDay()
    {
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.PreparingPhase);
    }
    public void TestRegisterOrder()
    {
        CustomerManager.Instance.CmdRegisterOrder();
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
       // CustomerManager.Instance.ServePotion(10000);
    }
}
