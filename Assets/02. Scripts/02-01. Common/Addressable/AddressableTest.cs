using Mirror;
using UnityEngine;

public class AddressableTest : NetworkBehaviour
{
    private void Start()
    {
        Invoke(nameof(AdressableLoadTest), 5f);
    }

    [Command]
    private void AdressableLoadTest()
    {
        TestFactory.Instance.Create(ETestType.Test1, transform.position, Quaternion.identity);
        TestFactory.Instance.Create(ETestType.Test2, transform.position, Quaternion.identity);
    }
}
