using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BasicMachineAnimation : MonoBehaviour, IMachineAnimation
{
    [SerializeField]
    private List<AnimationState> _animationState;
    [SerializeField]
    private List<GameObject> _ingredientObjectList;

    public void PutItemAnimation()
    {
    }

    public void EndAnimation()
    {
    }

    public void GetItemAnimation()
    {
        ResetAnimation();
    }

    public void ResetAnimation()
    {

    }

    public void StartAnimation()
    {
    }

    public void StopAnimation()
    {
    }
}
