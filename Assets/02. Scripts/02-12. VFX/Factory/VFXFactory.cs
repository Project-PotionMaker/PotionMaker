using Mirror;
using UnityEngine;

public class VFXFactory : NetworkFactoryBase<EVFXType, VFXFactoryInfo, VFXFactory>
{
    private void Start()
    {
        _factoryLogic.Initialize(_factoryInfoList, _poolParentObject).SafeFireAndForget();
    }

    [Server]
    public override GameObject CreateObject(EVFXType type, Vector3 position, Quaternion rotation)
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
