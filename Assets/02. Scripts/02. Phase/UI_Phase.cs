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
    [SerializeField] 
    private CanvasGroup _readyPanelGroup; 
    [SerializeField] 
    private CanvasGroup _alertPanelGroup;

    private const float READY_HIDE_OFFSET = 200f;
    private const float PLAYER_HIDE_OFFSET = 60f;
    private const float ALERT_HIDE_OFFSET = 100f;
    private const float WINDOW_WIDTH = 1920f;
    private const float WINDOW_HEIGHT = 1080f;
    private const float DURATION = 1f;
    private Sequence _alertSeq;
    private UI_VoteSystem _voteSystem;

    private void Start()
    {
        _voteSystem = GetComponent<UI_VoteSystem>();
        _voteSystem.enabled = false;
        _voteSystem.OnAlert += ShowAlert;
        _serviceTimer.maxValue = 1f;

        OnPhaseManagerInitialized();
        OnPlayerListManagerInitialized();
        ResetPlayerPanel();
    }

    private void OnPlayerListManagerInitialized()
    {
        PlayerListManager.Instance.OnPlayerListUpdated += ResetPlayerPanel;
    }

    private void OnPhaseManagerInitialized()
    {
        PhaseManager.Instance.OnDayPassed += UpdateDayText;
        PhaseManager.Instance.OnTimerRunning += UpdateServiceTimer;
        PhaseManager.Instance.OnDeathCountChanged += RefreshDeathCount;

        //GridManager.Instance.OnNotFoundPath += ShowAlert;

        UpdateDayText();
        RefreshDeathCount();

        PreparingPhase preparingPhase = (PreparingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.PreparingPhase];
        preparingPhase.OnPhaseEntered += ChangeTextStartDay;
        preparingPhase.OnPhaseEntered += StartVote;
        StartVote();

        PracticingPhase practicingPhase = (PracticingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase];
        practicingPhase.OnPhaseEntered += ChangeTextPracticeEnd;
        ServingPhase servingPhase = (ServingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase];

        servingPhase.OnPhaseEntered += ShowTimer; // 타이머 시작 시 업데이트
        servingPhase.OnPhaseExited += HideTimer;
        servingPhase.OnPhaseEntered += HideReady; // 준비 단계가 끝나면 시작 패널 숨김

        EndingPhase endingPhase = (EndingPhase)PhaseManager.Instance.PhaseDictionary[EPhaseType.EndingPhase];
        endingPhase.OnPhaseExited += ShowReady; // 준비 단계가 시작되면 시작 패널 표시
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
        _serviceTimer.GetComponent<RectTransform>().DOAnchorPosY(-ALERT_HIDE_OFFSET, DURATION).SetRelative().SetEase(Ease.OutSine).SetDelay(0.5f);
    }
    private void HideTimer()
    {
        _serviceTimer.GetComponent<RectTransform>().DOAnchorPosY(ALERT_HIDE_OFFSET, DURATION).SetRelative().SetEase(Ease.OutSine);
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
        PhaseManager.Instance.ServerTransitionPhase(EPhaseType.ServingPhase);
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
        const float fadeDur = 0.35f; // 패널 페이드 인/아웃 시간
        const float blinkDur = 0.3f;  // 한 번 깜빡임의 반 주기(투명 또는 불투명 전환 시간)

        _alertText.text = text;
        

        // 진행 중 트윈 정리
        _alertPanelGroup.DOKill();
        _alertText.DOKill();
        _alertSeq?.Kill();

        _alertPanelGroup.alpha = 0f;      // 패널은 보이지 않는 상태에서 시작

        _alertText.alpha = 0f;            // 텍스트는 보이는 상태에서 시작

        _alertSeq = DOTween.Sequence()
            .Append(_alertPanelGroup.DOFade(1f, fadeDur).SetEase(Ease.OutSine))
            .Append(_alertText.DOFade(1f, blinkDur))
            .Append(_alertText.DOFade(0f, blinkDur))
            .Append(_alertText.DOFade(1f, blinkDur))
            .Append(_alertText.DOFade(0f, blinkDur))
            .Append(_alertPanelGroup.DOFade(0f, fadeDur).SetEase(Ease.OutSine));
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
