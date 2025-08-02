using System;
using UnityEngine;

public enum Visibility
{
    Public, FriendOnly, Private
}

[Serializable]
public class RoomInfo
{
    public ShopInfo ShopInfo;
    public Visibility Visibility;

    public RoomInfo(ShopInfo shopInfo, Visibility visibility)
    {
        ShopInfo = shopInfo;
        Visibility = visibility;
    }
}
