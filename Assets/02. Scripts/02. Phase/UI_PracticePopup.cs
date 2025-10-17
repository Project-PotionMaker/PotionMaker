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
        VoteManager.Instance.OnVoteDone += EnterPracticingPhase;
        VoteManager.Instance.OnVoteDone += StopVote;
    }

    private void OnDisable()
    {
        _voteSystem.enabled = false;
        PopedDown?.Invoke();
    }
    private void StopVote()
    {
        _voteSystem.enabled = false;
        VoteManager.Instance.OnVoteDone -= EnterPracticingPhase;
        VoteManager.Instance.OnVoteDone -= StopVote;
        GameSceneUIManager.Instance.CloseAllPopups();
    }

    private void EnterPracticingPhase()
    {
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.PracticingPhase);
    }
}
