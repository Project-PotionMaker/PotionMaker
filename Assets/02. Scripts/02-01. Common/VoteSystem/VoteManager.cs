using Mirror;
using System;
using UnityEngine;

public class VoteManager : NetworkBehaviourSingleton<VoteManager>
{
    private const int MAX_PLAYER_COUNT = 4; // 최대 플레이어 수
    private bool[] _isVoted = new bool[MAX_PLAYER_COUNT];
    public bool[] IsVoted => _isVoted;
    [SyncVar]
    private bool _isVoting = false;
    public event Action OnVoteUpdated;
    public event Action OnVoteDone;
    public event Action StopVoting;
    public event Action OnVoteStarted;
    public event Action OnRefreshed;

    public override void OnStartClient()
    {
        base.OnStartClient();
        RefreshArray();
    }

    [ClientRpc]
    public void RpcRefreshArray()
    {
        RefreshArray();
    }

    public void RefreshArray()
    {
        for (int i = 0; i < MAX_PLAYER_COUNT; i++)
        {
            _isVoted[i] = false;
        }
        OnRefreshed?.Invoke();
    }

    [Command (requiresAuthority = false)]
    public void CmdVoting(int playerOrderIndex)
    {
        RpcVoting(playerOrderIndex);
    }

    [ClientRpc]
    public void RpcVoting(int playerOrderIndex)
    {
        _isVoted[playerOrderIndex] = !_isVoted[playerOrderIndex];
        OnVoteUpdated?.Invoke();
    }

    [Server]
    private void CheckDone()
    {
        if (PlayerListManager.Instance.PlayerNetIdList.Count == 0)
        {
            return;
        }
        bool nooneVoted = true;
        bool everyoneVoted = true;
        foreach (uint netId in PlayerListManager.Instance.PlayerNetIdList)
        {
            int index = NetworkServer.spawned[netId].GetComponent<Player>().PlayerOrderIndex;
            if(_isVoted[index])
            {
                nooneVoted = false;
            }
            else
            {
                everyoneVoted = false;
            }
        }
        if (nooneVoted)
        {
            RpcBroadCastStopVoting(); // 아무도 투표하지 않았음
        }
        if (everyoneVoted)
        {
            RpcBroadCastOnVoteDone(); // 모든 플레이어가 투표를 완료했음
        }
        return;
    }

    [ClientRpc]
    public void RpcBroadCastStopVoting()
    {
        StopVoting?.Invoke();
    }
    [ClientRpc]
    public void RpcBroadCastOnVoteDone()
    {
        OnVoteDone?.Invoke(); // 모든 플레이어가 투표를 완료했음
    }
    [ClientRpc]
    public void RpcBroadCastOnVoteStarted()
    {
        OnVoteStarted?.Invoke(); // 투표가 시작됨
    }

    public void SetVoteTime(bool voteTime)
    {
        if (isServer)
        {
            ServerSetVoteTime(voteTime);
        }
        else
        {
            if (voteTime == false)
            {
                return;
            }
            CmdSetVoteTime(voteTime);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetVoteTime(bool voteTime)
    {
        ServerSetVoteTime(voteTime);
    }

    [Server]
    public void ServerSetVoteTime(bool voteTime)
    {
        if (voteTime)
        {
            if (_isVoting)
            {
                return;
            }
            OnVoteUpdated += CheckDone;
            _isVoting = true;
            RpcBroadCastOnVoteStarted();
        }
        else
        {
            _isVoting = false;
            OnVoteUpdated -= CheckDone;
            RpcRefreshArray(); // 투표 시간 종료 시 투표 상태 초기화
        }
    }
}
