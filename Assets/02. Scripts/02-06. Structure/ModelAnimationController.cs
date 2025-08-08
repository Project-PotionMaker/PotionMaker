using UnityEngine;
using DG.Tweening;
using Mirror;
using NUnit.Framework;
using System.Collections.Generic;

public class ModelAnimationController : NetworkBehaviour
{
    private ModelAnimation _modelAnimation;

    public void InitModelAnimationController(GameObject modelObject)
    {
        _modelAnimation = modelObject.GetComponent<ModelAnimation>();
    }

    public void Play()
    {
        //_current
    }
}

public enum EMachineAnimationType
{
    Empty,
    Add,
    DoOnce,
    DoProgress,
    Done
}

public enum EMachineVisualType
{
    Animation,
    Mesh,
    Model,
    Effect
}
