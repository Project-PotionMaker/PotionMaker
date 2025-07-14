using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhaseManager : MonoBehaviourSingleton<PhaseManager>    
{
    private BasePhase _currentPhase;
    private PhotonView _photonView;
    public BasePhase CurrentPhase { get => _currentPhase; set => _currentPhase = value; }
    private Dictionary<EPhaseType, BasePhase> _phaseDictionary;
    public Dictionary<EPhaseType, BasePhase> PhaseDictionary { get => _phaseDictionary;}

    [SerializeField]
    private int _day;
    public int Day { get => _day; set => _day = value; }
    public event Action OnDayPassed;

    private void Start()
    {
        InitPhase();
    }

    private void Update()
    {
        _currentPhase?.Update(Time.deltaTime);
    }

    public void InitPhase()
    {
        //if(저장 데이터가 null이면)
        {
            _day = 1;
        }//else
        {
            //저장 데이터에서 _day를 불러오기
        }
        _phaseDictionary = new Dictionary<EPhaseType, BasePhase>
        {
            { EPhaseType.PreparingPhase, new PreparingPhase() },
            { EPhaseType.ServingPhase, new ServingPhase() },
            { EPhaseType.EndingPhase, new EndingPhase() },
        };
        _currentPhase = _phaseDictionary[EPhaseType.PreparingPhase];
        _currentPhase.EnterPhase();
    }

    public void TransitionPhase(EPhaseType nextPhase)
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        _photonView.RPC(nameof(RPC_TransitionPhase), RpcTarget.All, nextPhase);
    }
    [PunRPC]
    public void RPC_TransitionPhase(EPhaseType nextPhase)
    {
        if (_currentPhase != null && _currentPhase.PhaseType == nextPhase)
        {
            return; // 이미 같은 페이즈라면 중복 전이 방지
        }
        _currentPhase?.ExitPhase();
        if (_currentPhase is EndingPhase && _phaseDictionary[nextPhase] is PreparingPhase)
        {
            _day++;
            OnDayPassed?.Invoke();
        }
        _currentPhase = _phaseDictionary[nextPhase];
        _currentPhase.EnterPhase();
    }
}
