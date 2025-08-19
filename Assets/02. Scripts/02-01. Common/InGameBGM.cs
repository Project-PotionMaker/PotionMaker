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
        if(currentPhase == EPhaseType.PreparingPhase)
        {
            AudioManager.Instance.PlayBGM(EBGMAudioType.IngamePreparingPhase);
        }
        else if(currentPhase == EPhaseType.ServingPhase)
        {
            AudioManager.Instance.PlayBGM(EBGMAudioType.IngameServingPhase);
        }
    }
}
