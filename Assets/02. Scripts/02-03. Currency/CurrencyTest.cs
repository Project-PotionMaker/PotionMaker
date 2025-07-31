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
            CurrencyManager.Instance.CmdRequestAddCurrency(1000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrencyManager.Instance.TrySubtractCurrency(1000);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SalesManager.Instance.CmdRequestSell(EPotionType.Vitality, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SalesManager.Instance.CmdRequestSell(EPotionType.Life, 10);
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
