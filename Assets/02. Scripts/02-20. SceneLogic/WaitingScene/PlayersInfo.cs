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
    private List<UI_PlayerInfoSlot> _playerInfoSlotList;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        StartCoroutine(Refresh_Coroutine());
    }

    private IEnumerator Refresh_Coroutine()
    {
        // NetworkMessenger가 준비될 때까지 대기
        while (NetworkMessenger.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < _playerInfoSlotList.Count; i++)
        {
            _playerInfoSlotList[i].ClearSlot();
        }

        var currentPlayerIds = NetworkMessenger.Instance.RoomPlayerIdList.ToList();
        
        foreach (uint roomPlayerNetId in currentPlayerIds)
        {
            if (NetworkClient.spawned.TryGetValue(roomPlayerNetId, out NetworkIdentity identity))
            {
                RoomPlayer roomPlayer = identity.GetComponent<RoomPlayer>();
                if (roomPlayer != null)
                {
                    int slotNumber = roomPlayer.slotNumber;
                    if (slotNumber >= 0 && slotNumber < _playerInfoSlotList.Count)
                    {
                        Debug.Log($"PlayersInfo: Setting up slot {slotNumber} for player {roomPlayer.PlayerName} (Mirror index: {roomPlayer.index}, UI slot: {slotNumber})");
                        _playerInfoSlotList[slotNumber].InitPlayerInfoSlot(roomPlayer);
                    }
                    else
                    {
                        Debug.LogWarning($"PlayersInfo: Invalid slot number {slotNumber} for player {roomPlayer.PlayerName} (Mirror index: {roomPlayer.index})");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"PlayersInfo: Could not find RoomPlayer with NetId {roomPlayerNetId}");
            }
        }

        for (int i = 0; i < _playerInfoSlotList.Count; i++)
        {
            _playerInfoSlotList[i].Refresh();
        }
    }
}
