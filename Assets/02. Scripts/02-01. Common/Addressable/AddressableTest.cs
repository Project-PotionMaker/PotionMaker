using UnityEngine;

public class AddressableTest : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(AdressableLoadTest), 5f);
    }

    private void AdressableLoadTest()
    {
        TestFactory.Instance.Create(ETestType.Test1, transform.position, Quaternion.identity);
        TestFactory.Instance.Create(ETestType.Test2, transform.position, Quaternion.identity);
    }
}
