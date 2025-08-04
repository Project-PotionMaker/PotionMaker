using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkFactoryBase<TEnum, TFactoryInfo, TFactoryClass>
    : NetworkBehaviourSingleton<TFactoryClass>
    where TEnum : Enum
    where TFactoryInfo : BaseFactoryInfo<TEnum>
    where TFactoryClass : NetworkFactoryBase<TEnum, TFactoryInfo, TFactoryClass>
{
    [SerializeField]
    protected List<TFactoryInfo> _factoryInfoList;

    [SerializeField]
    protected Transform _poolParentObject;

    protected FactoryLogic<TEnum, TFactoryInfo> _factoryLogic = new FactoryLogic<TEnum, TFactoryInfo>();

    public abstract GameObject CreateObject(TEnum type, Vector3 position, Quaternion rotation);

    protected abstract void CmdReturnObject(GameObject obj);

    public void ReturnObject(GameObject obj)
    {
        CmdReturnObject(obj);
    }
}
