using UnityEngine;

public enum EAddressableTest
{
    AddressablePrefab,
    AddressableScene
}

public class AddressableInfo : BasePoolInfo<EAddressableTest>
{

}


public class TestPoolManager : BasePoolManager<EAddressableTest, BasePoolInfo<EAddressableTest>>
{
    
}
