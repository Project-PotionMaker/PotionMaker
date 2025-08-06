using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersInfo : MonoBehaviour
{
    [SerializeField]
    private List<UI_PlayerInfoSlot> PlayerInfoSlotList;

    public void Refresh()
    {
        StartCoroutine(Refresh_Coroutine());
    }

    private IEnumerator Refresh_Coroutine()
    {
        yield return WaitUntilAllRoomPlayersSpawned(NetworkMessenger.Instance.RoomPlayerIdList.ToList());

        foreach (uint roomPlayerNetId in NetworkMessenger.Instance.RoomPlayerIdList)
        {
            RoomPlayer roomPlayer = NetworkServer.spawned[roomPlayerNetId].GetComponent<RoomPlayer>();
            if (PlayerInfoSlotList[roomPlayer.index].CurrentRoomPlayer == roomPlayer)
            {
                PlayerInfoSlotList[roomPlayer.index].Refresh();
            }
            else
            {
                PlayerInfoSlotList[roomPlayer.index].InitPlayerInfoSlot(roomPlayer);
            }
        }

        foreach (UI_PlayerInfoSlot playerInfoSlot in PlayerInfoSlotList)
        {
            if (playerInfoSlot.CurrentRoomPlayer == null)
            {
                playerInfoSlot.Refresh();
            }
        }
    }

    private IEnumerator WaitUntilAllRoomPlayersSpawned(List<uint> netIds)
    {
        List<RoomPlayer> players = new List<RoomPlayer>();

        while (true)
        {
            players.Clear();
            bool allFound = true;

            foreach (uint id in netIds)
            {
                if (NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity))
                {
                    players.Add(identity.GetComponent<RoomPlayer>());
                }
                else
                {
                    allFound = false;
                    yield return new WaitForSeconds(0.1f);
                    break;
                }
            }

            if (allFound)
                break;

            yield return null; // 다음 프레임까지 대기
        }
    }
}
