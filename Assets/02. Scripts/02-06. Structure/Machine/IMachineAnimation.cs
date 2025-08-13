using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationState
{
    [SerializeField]
    public List<DOTweenAnimation> DotweenAnimationList;
    [SerializeField]
    public List<GameObject> ModelObjectList;
    [SerializeField]
    public List<ParticleSystem> ParticleSystemList;
}

public interface IMachineAnimation
{
    public void RefreshAnimation();
    public void StartAnimation();
    public void StopAnimation();
    public void RefreshModels();
}
