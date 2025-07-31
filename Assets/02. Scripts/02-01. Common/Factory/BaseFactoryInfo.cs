using System;

[Serializable]
public class BaseFactoryInfo<TEnum> where TEnum : Enum
{
    public TEnum Type;
    public EAddressableKeys AddressableKeyEnum;
    public string AddressableKey => AddressableKeyEnum.ToString();
}

