using Mirror;
using System;
using UnityEngine;

public abstract class NetworkFactoryBase<T> : NetworkBehaviourSingleton<NetworkFactoryBase<T>> where T : NetworkBehaviour
{
    [SerializeField]
    protected Transform _poolParentObject;

    public abstract GameObject CreateObject(Enum type, Vector3 position, Quaternion rotation);

    protected abstract void CmdReturnObject(GameObject obj);

    public void ReturnObject(GameObject obj)
    {
        CmdReturnObject(obj);
    }
}
