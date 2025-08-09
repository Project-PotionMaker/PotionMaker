using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using DG.Tweening;
using System;
using Mirror;
using System.Collections;

public class UI_Phase : MonoBehaviour
{
    [Foldout("UIs")]
    [SerializeField]
    private TextMeshProUGUI _dayText;
    [SerializeField]
    private Slider _serviceTimer;
    [SerializeField]
    private RectTransform _readyPanel;
    [SerializeField]
    private TextMeshProUGUI _startDayText;
    [SerializeField]
    private TextMeshProUGUI[] _playerName;
    [SerializeField]
    private RectTransform[] _playerPanel;
    [SerializeField]
    private GameObject[] _playerMask;
    [SerializeField]
    private Image[] _deathCountHeart;
    [SerializeField]
    private RectTransform _alertPanel;
    [SerializeField]
    private TextMeshProUGUI _alertText;

    private const float READY_HIDE_OFFSET = 200f;
    private const float PLAYER_HIDE_OFFSET = 60f;
    private const float ALERT_HIDE_OFFESET = 100f;
    private const float WINDOW_WIDTH = 1920f;
    private const float WINDOW_HEIGHT = 1080f;
    private const float DURATION = 1f;

    private UI_VoteSystem _voteSystem;

    private void Start()
    {
        _voteSystem = GetComponent<UI_VoteSystem>();
        _voteSystem.enabled = false;

        _serviceTimer.maxValue = 1f;
        PhaseManager.Instance.OnDayPassed += UpdateDayText;
        PhaseManager.Instance.OnTimerRunning += UpdateServiceTimer;
        PhaseManager.Instance.OnDeathCountChanged += RefreshDeathCount;

        UpdateDayText();
        RefreshDeathCount();

        PreparingPhase preparingPhase = (PreparingPhase) PhaseManager.Instance.PhaseDictionary[EPhaseType.PreparingPhase];
        preparingPhase.OnPhaseEntered += ChangeTextStartDay;
        preparingPhase.OnPhaseEntered += StartVote;
        StartVote();

        PracticingPhase practicingPhase = (PracticingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase];
        practicingPhase.OnPhaseEntered += ChangeTextPracticeEnd;
        ServingPhase servingPhase = (ServingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase];
        
        servingPhase.OnPhaseEntered += ShowTimer; // 타이머 시작 시 업데이트
        servingPhase.OnPhaseExited += HideTimer;
        servingPhase.OnPhaseEntered += HideReady; // 준비 단계가 끝나면 시작 패널 숨김
        HideTimer();
        EndingPhase endingPhase = (EndingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.EndingPhase];
        endingPhase.OnPhaseExited += ShowReady; // 준비 단계가 시작되면 시작 패널 표시

        PlayerListManager.Instance.OnPlayerListUpdated += ResetPlayerPanel;
        ResetPlayerPanel();
    }

    private void UpdateDayText()
    {
        if (_dayText != null)
        {
            _dayText.text = $"{PhaseManager.Instance.Day}일";
        }
    }
    private void UpdateServiceTimer()
    {
        _serviceTimer.value = 1-PhaseManager.Instance.CurrentTimeRate;
    }

    private void ShowTimer()
    {
        _serviceTimer.gameObject.SetActive(true);
    }
    private void HideTimer()
    {
        _serviceTimer.gameObject.SetActive(false);
    }

    private void HideReady()
    {
        Debug.Log("Hide Start Day Panel");
        _readyPanel.DOAnchorPosY(READY_HIDE_OFFSET, DURATION).SetRelative().SetEase(Ease.OutSine);
    }
    private void ShowReady()
    {
        _readyPanel.DOAnchorPosY(-READY_HIDE_OFFSET, DURATION).SetRelative().SetEase(Ease.OutSine);
        
    }
    private void StartVote()
    {
        if (VoteManager.Instance == null)
        {
            Debug.LogWarning("VoteManager not ready yet. Delaying StartVote.");
            StartCoroutine(WaitAndStartVote());
            return;
        }
        _voteSystem.enabled = true;
        VoteManager.Instance.OnVoteDone += NextPhase;
        VoteManager.Instance.OnVoteDone += StopVote;
    }
    private IEnumerator WaitAndStartVote()
    {
        while (VoteManager.Instance == null)
            yield return null;

        StartVote(); // 재시도
    }
    private void StopVote()
    {
        _voteSystem.enabled = false;
        VoteManager.Instance.OnVoteDone -= NextPhase;
        VoteManager.Instance.OnVoteDone -= StopVote;
    }

    private void NextPhase()
    {
        if(NetworkServer.active == false)
        {
            return;
        }
        PhaseManager.Instance.TransitionPhase(EPhaseType.ServingPhase);
    }

    private void ResetPlayerPanel()
    {
        for(int index = 0; index < 4; index++)
        {
            _playerMask[index].SetActive(true);
            _playerPanel[index].DOAnchorPosY(-PLAYER_HIDE_OFFSET, DURATION).SetEase(Ease.OutSine);
        }

        foreach(uint netId in PlayerListManager.Instance.PlayerNetIdList)
        {
            Player player = NetworkClient.spawned[netId].GetComponent<Player>();
            int index = player.PlayerOrderIndex;
            _playerName[index].text =player.playerName;
            _playerMask[index].SetActive(false);
            _playerPanel[index].transform.DOKill();
            _playerPanel[index].DOAnchorPosY(0, DURATION).SetEase(Ease.OutSine);
        }
    }

    private void ChangeTextStartDay()
    {
        _startDayText.text = "영업 시작";
    }
    private void ChangeTextPracticeEnd()
    {
        _startDayText.text = "연습 종료";
    }
    private void RefreshDeathCount()
    {
        for (int i = 0; i < _deathCountHeart.Length; i++)
        {
            _deathCountHeart[i].color = i < PhaseManager.Instance.DeathCount ? Color.white : Color.black;
        }
    }
    public void ShowAlert(string text)
    {
        const float showOffsetY = -ALERT_HIDE_OFFESET;
        const float hideOffsetY = ALERT_HIDE_OFFESET;
        const float stayDuration = 2f;

        // 텍스트 세팅
        _alertText.text = text;

        _alertPanel.DOAnchorPosY(showOffsetY, DURATION)
            .SetRelative()
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                // 2초 후 사라지기
                DOVirtual.DelayedCall(stayDuration, () =>
                {
                    _alertPanel.DOAnchorPosY(hideOffsetY, DURATION)
                        .SetRelative()
                        .SetEase(Ease.OutSine);
                });
            });
    }



    public void OptionPanelShow()
    {
        //OptionPanel.SerActive();
    }

    public void aaa()//도감 팝업
    {
    }//TODO : 도감 팝업 띄우기


    //TODO : 준비 키 키세팅 따라가기
}
