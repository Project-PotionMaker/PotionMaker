using UnityEngine;

public class Test_MarketSingleton : MonoBehaviourSingleton<Test_MarketSingleton>
{
    public GameObject Market;
    public void ShowHideMarket()
    {
        Market.SetActive(!Market.activeSelf);
    }
}
