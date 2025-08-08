using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UI_VoteSystem : MonoBehaviour
{
    [SerializeField]
    private Image[] _voteBackground;
    [SerializeField]
    private Image[] _checkIcon;

    private int index = -1;

    private void Awake()
    {
        foreach(Image background in _voteBackground)
        {
            background.enabled = false;
        }
        foreach (Image checkIcon in _checkIcon)
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
        VoteManager.Instance.SetVoteTime(true);
        Refresh();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(index == -1)
            {
                index = NetworkClient.localPlayer.GetComponent<Player>().PlayerOrderIndex;
            }
            VoteManager.Instance.CmdVoting(index);
            Debug.Log(gameObject);
        }
    }

    private void Refresh()
    {
        Debug.Log($"{gameObject} Refresh");
        if (PlayerListManager.Instance.PlayerNetIdList.Count == 0)
        {
            return;
        }
        foreach (Image background in _voteBackground)
        {
            background.enabled = false;
        }
        foreach (uint netId in PlayerListManager.Instance.PlayerNetIdList)
        {
            int index = NetworkClient.spawned[netId].GetComponent<Player>().PlayerOrderIndex;
            _voteBackground[index].enabled = true;
            if (VoteManager.Instance.IsVoted[index])
            {
                _checkIcon[index].enabled = true;
            }
            else
            {
                _checkIcon[index].enabled = false;
            }
        }
    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject} OnDisable");
        foreach (Image background in _voteBackground)
        {
            background.enabled = false;
        }
        foreach (Image checkIcon in _checkIcon)
        {
            checkIcon.enabled = false;
        }

        VoteManager.Instance.OnVoteUpdated -= Refresh;
        PlayerListManager.Instance.OnPlayerListUpdated -= Refresh;
        VoteManager.Instance.SetVoteTime(false);
    }


}
