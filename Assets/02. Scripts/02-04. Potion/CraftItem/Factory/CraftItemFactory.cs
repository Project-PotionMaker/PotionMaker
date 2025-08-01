using Mirror;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CraftItemFactory : NetworkFactoryBase<CraftItemFactory>
{
    [SerializeField]
    private List<CraftItemFactoryInfo> _factoryInfoList;

    private FactoryLogic<EInputType, CraftItemFactoryInfo> _factoryLogic = new FactoryLogic<EInputType, CraftItemFactoryInfo>();


    private void Start()
    {
        _factoryLogic.Initialize(_factoryInfoList);
    }

    [Server]
    public override GameObject CreateObject(Enum type, Vector3 position, Quaternion rotation)
    {
        if (type is EInputType inputType)
        {
            GameObject networkObject = _factoryLogic.GetObject(inputType, position, rotation);
            if (networkObject != null)
            {
                NetworkServer.Spawn(networkObject);
            }

            return networkObject;
        }

        Debug.LogError("스폰하려는 오브젝트의 type이 EInputType 아닙니다.");
        return null;
    }


    [Command(requiresAuthority = false)]
    protected override void CmdReturnObject(GameObject obj)
    {
        NetworkServer.UnSpawn(obj);
        _factoryLogic.ReturnObject(obj);
    }
}
