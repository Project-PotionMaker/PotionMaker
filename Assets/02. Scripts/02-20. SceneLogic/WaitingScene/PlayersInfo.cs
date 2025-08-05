using Mirror;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInfo : MonoBehaviour
{
    [SerializeField]
    private List<UI_PlayerInfoSlot> PlayerInfoSlotList;

    private void Awake()
    {
        MirrorNetworkManager.Instance.OnWaitingScenePlayerAdded += Refresh;
    }

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
    }

    private void OnDisable()
    {
        MirrorNetworkManager.Instance.OnWaitingScenePlayerAdded -= Refresh;
    }
}
