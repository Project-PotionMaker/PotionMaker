using UnityEngine;

public class RoomInfoHandler : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private ShopInfoHandler _shopInfoHandler;

    private RoomInfo _roomInfo;
    public RoomInfo RoomInfo => _roomInfo;

    private Visibility _selectedVisibilty = Visibility.Public;

    private void Start()
    {
        _shopInfoHandler.OnShopInfoUpdated += UpdateRoomInfo;
    }

    public void UpdateRoomInfo(ShopInfo shopInfo)
    {
        if (_roomInfo == null)
        {
            _roomInfo = new RoomInfo(shopInfo, _selectedVisibilty);
        }
        else
        {
            _roomInfo.ShopInfo = shopInfo;
        }
    }

    public void UpdateRoomInfo(Visibility visibilty)
    {
        if (_roomInfo == null)
        {
            _roomInfo = new RoomInfo(_shopInfoHandler.SelectedShopInfo, _selectedVisibilty);
        }
        else
        {
            _roomInfo.Visibility = visibilty;
        }
    }
    
}
