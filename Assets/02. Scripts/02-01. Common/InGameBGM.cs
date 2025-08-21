using UnityEngine;

public class InGameBGM : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayBGM(EBGMAudioType.IngamePreparingPhase);
        PhaseManager.Instance.OnPhaseChanged += ChangeBGM;
    }

    private void ChangeBGM()
    {
        EPhaseType currentPhase = PhaseManager.Instance.CurrentPhase.PhaseType;
        switch (currentPhase)
        {
            case EPhaseType.PreparingPhase:
                AudioManager.Instance.PlayBGM(EBGMAudioType.IngamePreparingPhase);
                break;
            case EPhaseType.ServingPhase:
                AudioManager.Instance.PlayBGM(EBGMAudioType.IngameServingPhase);
                break;
        }
    }

    private void OnDestroy()
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.OnPhaseChanged -= ChangeBGM;
        }
    }
}
