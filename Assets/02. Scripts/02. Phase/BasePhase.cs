using System;
using UnityEngine;

public abstract class BasePhase
{
    protected EPhaseType _phaseType;
    public EPhaseType PhaseType { get => _phaseType; set => _phaseType = value; }

    public Action OnPhaseEntered;
    public Action OnPhaseExited;

    public virtual void EnterPhase()
    {
        Debug.Log($"{PhaseType} phase entered.");
        OnPhaseEntered?.Invoke();
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void ExitPhase()
    {
        Debug.Log($"{PhaseType} phase exited.");
        OnPhaseExited?.Invoke();
    }
}
