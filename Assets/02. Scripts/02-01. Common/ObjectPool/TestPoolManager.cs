using UnityEngine;

public enum EAddressableTest
{
    AddressablePrefab1,
    AddressablePrefab2,
    AddressableScene
}

public class AddressableInfo : BasePoolInfo<EAddressableTest>
{

}


public class TestPoolManager : BasePoolManager<EAddressableTest, BasePoolInfo<EAddressableTest>>
{
    
}
