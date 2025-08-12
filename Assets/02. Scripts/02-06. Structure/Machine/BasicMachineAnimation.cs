using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BasicMachineAnimation : MonoBehaviour, IMachineAnimation
{
    [SerializeField]
    private List<AnimationState> _animationState;

    [SerializeField]
    private List<ModelOnTID> _ingredientObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _ingredientObjectDic;

    private Machine _owner;

    private void Awake()
    {
        _owner = GetComponent<Machine>();
        _ingredientObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _ingredientObjectList)
        {
            modelInfo.Model.SetActive(false);
            _ingredientObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
    }

    public void PutItemAnimation()
    {
        foreach(ModelOnTID ingredient in _ingredientObjectList)
        {
            ingredient.Model.SetActive(false);
        }
        foreach(int tid in _owner.InputTIDList)
        {
            _ingredientObjectDic[tid].SetActive(true);
        }
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
