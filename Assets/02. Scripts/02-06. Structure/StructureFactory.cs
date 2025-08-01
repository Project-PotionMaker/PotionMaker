using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureFactory : NetworkFactoryBase<StructureFactory>
{
    [SerializeField]
    private List<StructureFactoryInfo> _factoryInfoList;

    private FactoryLogic<EStructureType, StructureFactoryInfo> _factoryLogic = new FactoryLogic<EStructureType, StructureFactoryInfo>();


    private void Start()
    {
        _factoryLogic.Initialize(_factoryInfoList);
    }

    [Server]
    public override GameObject CreateObject(Enum type, Vector3 position, Quaternion rotation)
    {
        if (type is EStructureType structureType)
        {
            GameObject networkObject = _factoryLogic.GetObject(structureType, position, rotation);
            if (networkObject != null)
            {
                NetworkServer.Spawn(networkObject);
            }

            return networkObject;
        }

        Debug.LogError("스폰하려는 오브젝트의 type이 EStructureType이 아닙니다.");
        return null;
    }


    [Command(requiresAuthority = false)]
    protected override void CmdReturnObject(GameObject obj)
    {
        NetworkServer.UnSpawn(obj);
        _factoryLogic.ReturnObject(obj);
    }
}