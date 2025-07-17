using Photon.Pun;
using UnityEngine;

public class CurrencyTest : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrencyManager.Instance.RequestAddCurrency(1000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrencyManager.Instance.TrySubtractCurrency(1000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrencyManager.Instance.PhotonView.RPC(nameof(CurrencyManager.SetCurrency), RpcTarget.All, 9999);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SalesManager.Instance.RequestSell(EPotionType.Vitality, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SalesManager.Instance.RequestSell(EPotionType.Life, 10);
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
}
