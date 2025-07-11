using UnityEngine;

public abstract class BasePhase
{
    protected EPhaseType _phaseType;
    public EPhaseType PhaseType { get => _phaseType; set => _phaseType = value; }

    public virtual void EnterPhase()
    {
        Debug.Log($"{PhaseType} phase entered.");
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void ExitPhase()
    {
        Debug.Log($"{PhaseType} phase exited.");
    }
}
