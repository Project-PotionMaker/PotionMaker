using Mirror;
using UnityEngine;

public class CraftItemFactory : NetworkFactoryBase<EInputType, CraftItemFactoryInfo, CraftItemFactory>
{
    private void Start()
    {
        _factoryLogic.Initialize(_factoryInfoList, _poolParentObject);
    }

    [Server]
    public override GameObject CreateObject(EInputType type, Vector3 position, Quaternion rotation)
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
