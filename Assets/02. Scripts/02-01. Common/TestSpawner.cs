using Mirror;
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TestSpawner : NetworkBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RequestCreate(ETestType.Test1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RequestCreate(ETestType.Test2);
        }
    }

    public void RequestCreate(ETestType type)
    {
        CmdCreate(type);
    }

    [Command]
    private void CmdCreate(ETestType type)
    {
        GameObject newObject = TestFactory.Instance.Create(type, transform.position, transform.rotation);

        Response(connectionToClient, newObject);
    }

    [TargetRpc]
    public void Response(NetworkConnection target, GameObject newObject)
    {
        
    }
}
