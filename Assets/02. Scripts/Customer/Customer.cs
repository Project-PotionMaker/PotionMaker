using UnityEngine;
using Photon.Pun;

public class Customer : MonoBehaviour
{
    private int _requestedPotionTID;
    public int RequestedPotionTID { get => _requestedPotionTID; set=> _requestedPotionTID = value; } // 요청한 포션 ID

    private PhotonView _photonView;

    public void SetCanInteract(bool canInteract)
    {
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { $"NPC_{_photonView.ViewID}_CanInteract", canInteract }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
