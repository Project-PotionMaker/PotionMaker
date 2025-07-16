using System;
using UnityEngine;

public enum ETestType
{
    Test1,
    Test2
}

[Serializable]
public class TestFactoryInfo : BaseFactoryInfo<ETestType>
{

}

public class TestFactory : BaseFactory<ETestType, TestFactoryInfo>
{
    
}
