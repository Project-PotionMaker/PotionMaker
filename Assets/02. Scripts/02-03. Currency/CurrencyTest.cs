using UnityEngine;

public class CurrencyTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrencyManager.Instance.AddCurrency(1000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrencyManager.Instance.TrySubtractCurrency(1000);
        }
    }
}
