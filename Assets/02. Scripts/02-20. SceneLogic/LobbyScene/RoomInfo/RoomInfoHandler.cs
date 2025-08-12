using UnityEngine;

public class RoomInfoHandler : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private ShopInfoHandler _shopInfoHandler;
    public ShopInfoHandler ShopInfoHandler => _shopInfoHandler;

    private RoomInfo _roomInfo;
    public RoomInfo RoomInfo => _roomInfo;

    private Visibility _selectedVisibility = Visibility.Public;

    private void Awake()
    {
        _shopInfoHandler.OnShopInfoUpdated += UpdateRoomInfo;
    }

    public void UpdateRoomInfo(ShopInfo shopInfo)
    {
        if (_roomInfo == null)
        {
            _roomInfo = new RoomInfo(shopInfo, _selectedVisibility);
        }
        else
        {
            _roomInfo.ShopInfo = shopInfo;
        }
    }

    public void UpdateRoomInfo(Visibility visibility)
    {
        if (_roomInfo == null)
        {
            _roomInfo = new RoomInfo(_shopInfoHandler.SelectedShopInfo, _selectedVisibility);
        }
        else
        {
            _roomInfo.Visibility = visibility;
        }
    }
    
}
