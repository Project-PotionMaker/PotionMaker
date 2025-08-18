using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UI_VoteSystem : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _voteBackgroundList;
    [SerializeField]
    private List<Image> _checkIconList;

    private int index = -1;

    private void Awake()
    {
        foreach(GameObject background in _voteBackgroundList)
        {
            background.SetActive(false);
        }
        foreach (Image checkIcon in _checkIconList)
        {
            checkIcon.enabled = false;
        }
        VoteManager.Instance.OnRefreshed += Refresh;
    }

    private void OnEnable()
    {
        Debug.Log($"{gameObject} OnEnable");
        VoteManager.Instance.OnVoteUpdated += Refresh;
        PlayerListManager.Instance.OnPlayerListUpdated += Refresh;
        InputManager.Instance.OnReadyEvent += Vote;
        VoteManager.Instance.SetVoteTime(true);
        Refresh();
    }
    private void OnDisable()
    {
        Debug.Log($"{gameObject} OnDisable");
        foreach (GameObject background in _voteBackgroundList)
        {
            background.SetActive(false);
        }
        foreach (Image checkIcon in _checkIconList)
        {
            checkIcon.enabled = false;
        }

        VoteManager.Instance.OnVoteUpdated -= Refresh;
        PlayerListManager.Instance.OnPlayerListUpdated -= Refresh;
        InputManager.Instance.OnReadyEvent -= Vote;
        VoteManager.Instance.SetVoteTime(false);
    }

    private void OnDestroy()
    {
        VoteManager.Instance.OnRefreshed -= Refresh;
    }
    private void Vote()
    {
        if(index == -1)
        {
            index = NetworkClient.localPlayer.GetComponent<Player>().PlayerOrderIndex;
        }
        VoteManager.Instance.CmdVoting(index);
        Debug.Log(gameObject);
    }

    private void Refresh()
    {
        Debug.Log($"{gameObject} Refresh");
        if (PlayerListManager.Instance.PlayerNetIdList.Count == 0)
        {
            return;
        }
        foreach (GameObject background in _voteBackgroundList)
        {
            background.SetActive(false);
        }
        foreach (uint netId in PlayerListManager.Instance.PlayerNetIdList)
        {
            int index = NetworkClient.spawned[netId].GetComponent<Player>().PlayerOrderIndex;
            _voteBackgroundList[index].SetActive(true);
            if (VoteManager.Instance.IsVoted[index])
            {
                _checkIconList[index].enabled = true;
            }
            else
            {
                _checkIconList[index].enabled = false;
            }
        }
    }

}
