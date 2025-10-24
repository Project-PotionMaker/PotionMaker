using Mirror;
using System;
using UnityEngine;

public class UI_PracticePopup : MonoBehaviour
{
    private UI_VoteSystem _voteSystem;

    public event Action PopedUp;
    public event Action PopedDown;
    private void Awake()
    {
        _voteSystem = GetComponent<UI_VoteSystem>();
    }

    private void OnEnable()
    {
        PopedUp?.Invoke();
        _voteSystem.enabled = true;
        if (NetworkServer.active) 
        {
            VoteManager.Instance.OnVoteDone += EnterPracticingPhase;
            VoteManager.Instance.OnVoteDone += StopVote;
        }

    }

    private void OnDisable()
    {
        StopVote();
        PopedDown?.Invoke();
    }

    private void StopVote()
    {
        Debug.Log("호출횟수카운트용");
        _voteSystem.enabled = false;
        if(NetworkServer.active)
        {
            VoteManager.Instance.OnVoteDone -= EnterPracticingPhase;
            VoteManager.Instance.OnVoteDone -= StopVote;
        }

        PopupSyncronizer.Instance.CloseAllPopupsSynced();
    }

    private void EnterPracticingPhase()
    {
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.PracticingPhase);
    }
}
