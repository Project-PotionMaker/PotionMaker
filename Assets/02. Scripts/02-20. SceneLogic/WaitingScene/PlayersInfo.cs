using Mirror;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInfo : MonoBehaviour
{
    [SerializeField]
    private List<UI_PlayerInfoSlot> PlayerInfoSlotList;

    private Dictionary<uint, UI_PlayerInfoSlot> _activePlayerUIMap = new Dictionary<uint, UI_PlayerInfoSlot>();


    public void Refresh()
    {
        HashSet<NetworkRoomPlayer> roomslots = MirrorNetworkManager.Instance.roomSlots;

        foreach (RoomPlayer roomPlayer in roomslots)
        {
            if (PlayerInfoSlotList[roomPlayer.index].CurrentRoomPlayer == roomPlayer)
            {
                PlayerInfoSlotList[roomPlayer.index].Refresh();
            }
            else
            {
                PlayerInfoSlotList[roomPlayer.index].InitPlayerInfoSlot(roomPlayer);
            }
        }

        foreach(UI_PlayerInfoSlot playerInfoSlot in PlayerInfoSlotList)
        {
            if(playerInfoSlot.CurrentRoomPlayer == null)
            {
                playerInfoSlot.Refresh();
            }
        }
    }
}
