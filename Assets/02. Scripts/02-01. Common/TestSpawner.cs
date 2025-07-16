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

    public void RequestCreate(ETestType type)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(Create), RpcTarget.MasterClient, type);
            return;
        }
        Photon.Realtime.Player sender = PhotonNetwork.LocalPlayer;
        int timestamp = PhotonNetwork.ServerTimestamp;

        Create(type, new PhotonMessageInfo(sender, timestamp, _photonView));
    }

    [PunRPC]
    public void Create(ETestType type, PhotonMessageInfo info)
    {
        Debug.Log(PhotonNetwork.MasterClient.NickName);
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        GameObject newObject = TestFactory.Instance.Create(type, transform.position, transform.rotation);
        PhotonView targetPhotonView = newObject.GetComponent<PhotonView>();
        if (targetPhotonView == null)
        {
            return;
        }

        int viewID = targetPhotonView.ViewID;
        if (info.Sender.IsMasterClient)
        {
            Response(viewID);
        }
        else
        {
            _photonView.RPC(nameof(Response), info.Sender, viewID);
        }
    }

    [PunRPC]
    public void Response(int viewID)
    {
        GameObject newObject = PhotonView.Find(viewID).gameObject;
        Debug.Log($"{newObject.name}");
    }
}
