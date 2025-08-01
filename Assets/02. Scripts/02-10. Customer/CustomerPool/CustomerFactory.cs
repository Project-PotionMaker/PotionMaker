using Mirror;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CustomerFactory : NetworkFactoryBase<CustomerFactory>
{
    [SerializeField]
    private List<CustomerFactoryInfo> _factoryInfoList;

    private FactoryLogic<ENPCType, CustomerFactoryInfo> _factoryLogic = new FactoryLogic<ENPCType, CustomerFactoryInfo>();


    private void Start()
    {
        _factoryLogic.Initialize(_factoryInfoList);
    }

    [Server]
    public override GameObject CreateObject(Enum type, Vector3 position, Quaternion rotation)
    {
        if (type is ENPCType npcType)
        {
            GameObject networkObject = _factoryLogic.GetObject(npcType, position, rotation);
            if (networkObject != null)
            {
                NetworkServer.Spawn(networkObject);
            }

            return networkObject;
        }

        Debug.LogError("스폰하려는 오브젝트의 type이 ENPCType 아닙니다.");
        return null;
    }


    [Command(requiresAuthority = false)]
    protected override void CmdReturnObject(GameObject obj)
    {
        NetworkServer.UnSpawn(obj);
        _factoryLogic.ReturnObject(obj);
    }
}
