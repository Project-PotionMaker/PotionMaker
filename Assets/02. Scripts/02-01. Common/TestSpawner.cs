using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TestSpawner : MonoBehaviour
{
    private PhotonView _photonView;
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

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

    public async void RequestCreate(ETestType type)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(Create), RpcTarget.MasterClient, type);
            return;
        }

        await Create(type);
    }

    public async Task<GameObject> Create(ETestType type)
    {
        GameObject newObject = await TestFactory.Instance.CreateAsync(type, transform.position, transform.rotation);
        return newObject;
    }

    [PunRPC]
    public async void Create(ETestType type, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        GameObject newObject = await Create(type);
        PhotonView targetPhotonView = newObject.GetComponent<PhotonView>();
        if (targetPhotonView == null)
        {
            return;
        }

        _photonView.RPC(nameof(Response), info.Sender, targetPhotonView.ViewID);
    }

    [PunRPC]
    public void Response(int viewID)
    {
        GameObject newObject = PhotonView.Find(viewID).gameObject;
        Debug.Log($"{newObject.name}");
    }
}
