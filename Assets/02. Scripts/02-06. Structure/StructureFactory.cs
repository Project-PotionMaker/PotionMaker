using Mirror;
using UnityEngine;

public class StructureFactory : NetworkFactoryBase<EStructureType, StructureFactoryInfo, StructureFactory>
{
    private void Start()
    {
        _factoryLogic.Initialize(_factoryInfoList, _poolParentObject);   
    }


    [Server]
    public override GameObject CreateObject(EStructureType type, Vector3 position, Quaternion rotation)
    {
        GameObject networkObject = _factoryLogic.GetObject(type, position, rotation);

        if (networkObject != null)
        {
            NetworkServer.Spawn(networkObject);
        }

        return networkObject;
    }


    [Command(requiresAuthority = false)]
    protected override void CmdReturnObject(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        
        NetworkServer.UnSpawn(obj);
    }
}