using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class UI_VoteSystem : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _voteBackgroundList;
    [SerializeField]
    private List<Image> _checkIconList;

    private int index = -1;
    public event Action<string> OnAlert;
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
        Player player;
        player = NetworkClient.localPlayer.GetComponent<Player>();
        if (index == -1)
        {
            index = player.PlayerOrderIndex;
        }
        if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase && player.GetAbility<PlayerPickupAbility>().HeldItemIdentity != null)
        {
            OnAlert?.Invoke("우선 배치를 끝내세요");
            return;
        }
        if(GridManager.Instance.HasPath == false)
        {
            OnAlert?.Invoke("가구를 다시 배치하세요");
            return;
        }
        if(GridManager.Instance.IsEmptyArea(EAreaType.FrontYard) == false)
        {
            OnAlert?.Invoke("확인하지 않은 택배가 있습니다");
            return;
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
