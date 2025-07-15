using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BasePoolInfo<TEnum> where TEnum : Enum
{
    public TEnum Type;
    public int InitCount;
    public EAddressables AddressableKeyEnum;
    public string AddressableKey => AddressableKeyEnum.ToString();
    public Transform Container;

    public Queue<GameObject> PoolQueue = new Queue<GameObject>();
}