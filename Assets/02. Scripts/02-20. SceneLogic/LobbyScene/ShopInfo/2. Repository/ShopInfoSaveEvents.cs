using System;
using UnityEngine;

public static class ShopInfoSaveEvents
{
    public static event Action<ShopInfo> OnSaveRequest;

    public static void TriggerSaveRequest(ShopInfo shopInfo)
    {
        OnSaveRequest?.Invoke(shopInfo);
    }
}
