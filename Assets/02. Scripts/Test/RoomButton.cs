using TMPro;
using UnityEngine;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _roomTextUI;
    private string _address;
    
    public void Refresh(string address)
    {
        _address = address;
    }

    public void OnClickButton()
    {
        MirrorNetworkManager.Instance.networkAddress = _address;
        MirrorNetworkManager.Instance.StartClient();
    }
}
