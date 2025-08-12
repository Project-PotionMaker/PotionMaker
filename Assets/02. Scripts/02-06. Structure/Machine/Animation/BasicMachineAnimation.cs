using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicMachineAnimation : MonoBehaviour, IMachineAnimation
{
    [SerializeField]
    private List<DOTweenAnimation> _dotweenAnimationList;

    [SerializeField]
    private List<ModelOnTID> _ingredientObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _ingredientObjectDic;

    [SerializeField]
    private ColorOnType _outputObject; 
    private MaterialPropertyBlock _mpb;

    private Machine _owner;

    private float _lastProgress;

    private void Awake()
    {
        _owner = GetComponentInParent<Machine>();
        _ingredientObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _ingredientObjectList)
        {
            modelInfo.Model.SetActive(false);
            _ingredientObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
        _outputObject?.TypeObject.SetActive(false);
        _mpb = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        _owner.OnDataChanged += RefreshAnimation;
    }
    private void OnDisable()
    {
        _owner.OnDataChanged -= RefreshAnimation;
    }
    public void RefreshAnimation()
    {
        RefreshModels();

        if(_owner.CurrentProgress > _lastProgress && _owner.IsProcessFinished == false)
        {
            _lastProgress = _owner.CurrentProgress;
            StartAnimation();
        }
        if(_owner.CurrentProgress < _lastProgress)
        {
            _lastProgress = _owner.CurrentProgress;
        }
    }

    public void RefreshModels()
    {
        foreach (ModelOnTID ingredient in _ingredientObjectList)
        {
            ingredient.Model.SetActive(false);
        }

        if (_owner.IsProcessFinished && _owner.LeftOutputAmount > 0 && _outputObject != null)
        {
            _outputObject.TypeObject.SetActive(true);
            int outputTID = CraftItemManager.Instance.GetOutputTID(_owner.InputTIDList.ToArray(), _owner.DataTID, _owner.InputType);
            OutputData outputData = DataTable.Instance.GetOutputData(outputTID);
            
            if (ColorUtility.TryParseHtmlString(outputData.ColorCode, out Color parsedColor))
            {
                _mpb.SetColor("_BaseColor", parsedColor);
            }
            else
            {
                _mpb.SetColor("_BaseColor", Color.white);
            }

            _outputObject.ColorChangeRenderer.SetPropertyBlock(_mpb);
            _outputObject.TypeObject.SetActive(true);
        }
        else
        {
            _outputObject?.TypeObject.SetActive(false);
            foreach (int tid in _owner.InputTIDList)
            {
                if (_ingredientObjectDic.TryGetValue(tid, out GameObject ingredientObject))
                {
                    ingredientObject?.SetActive(true);
                }
            }
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
        foreach (DOTweenAnimation anime in _dotweenAnimationList)
        {
            if(anime.tween == null)
            {
                anime.CreateTween();
            }
            anime.DORestart();
        }
    }

    public void StopAnimation()
    {
    }

}
