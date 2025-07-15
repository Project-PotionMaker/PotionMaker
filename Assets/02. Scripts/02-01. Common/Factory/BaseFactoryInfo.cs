using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class BaseFactoryInfo<TEnum> where TEnum : Enum
{
    public TEnum Type;
    public EAddressables AddressableKeyEnum;
    public string AddressableKey => AddressableKeyEnum.ToString();
}

