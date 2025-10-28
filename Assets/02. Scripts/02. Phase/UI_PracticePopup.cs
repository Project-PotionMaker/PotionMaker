using Mirror;
using System;
using UnityEngine;

public class UI_PracticePopup : MonoBehaviour
{
    private UI_VoteSystem _voteSystem;

    public event Action OnPoppedUp;
    public event Action OnPoppedDown;
    private void Awake()
    {
        _voteSystem = GetComponent<UI_VoteSystem>();
    }

    private void OnEnable()
    {
        OnPoppedUp?.Invoke();
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
        OnPoppedDown?.Invoke();
    }

    private void StopVote()
    {
        _voteSystem.enabled = false;
        if (NetworkServer.active)
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
