//using Photon.Pun;
using UnityEngine;

public class CurrencyTest : MonoBehaviour
{
    public GameObject UI_Market;
    private void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
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
            SalesManager.Instance.RequestSell(10000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SalesManager.Instance.RequestSell(10001);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ProductManager.Instance.CmdRequestUnlock(10000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ProductManager.Instance.CmdRequestUnlock(10001);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            UI_Market.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SalesManager.Instance.CmdRequestUpdateSales(true);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            RentManager.Instance.CmdRequestPayRent();
        }
    }

    //public override void OnConnectedToMaster()
    //{
    //    PhotonNetwork.JoinLobby();
    //}

    //public override void OnJoinedLobby()
    //{
    //    PhotonNetwork.JoinRandomOrCreateRoom();
    //}
}
