using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineAnimation : MonoBehaviour, IMachineAnimation
{
    [SerializeField]
    private List<DOTweenAnimation> _dotweenOnceAnimationList;
    [SerializeField]
    private List<DOTweenAnimation> _dotweenLoopAnimationList;

    [SerializeField]
    private List<EVFXType> _clickParticleList;
    [SerializeField]
    private List<ParticleSystem> _loopParticleList;
    [SerializeField]
    private List<ParticleSystem> _doneParticleList;


    [SerializeField]
    private List<ModelOnTID> _ingredientObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _ingredientObjectDic;

    [SerializeField]
    private ColorOnType _outputObject;
    [SerializeField]
    private GameObject _baseObject;
    private MaterialPropertyBlock _mpb;

    private Machine _owner;
    private bool _lastProcessStarted = false;
    private bool _lastProcessFinished = false;
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
        if(_outputObject.TypeObject != null)
        {
            _outputObject.TypeObject.SetActive(false);
            _mpb = new MaterialPropertyBlock();
        }
        if(_baseObject != null)
        {
            _baseObject.SetActive(false);
        }

        foreach (var particle in _loopParticleList)
        {
            particle.Stop();
        }
        foreach (var particle in _doneParticleList)
        {

            particle.Stop();
            
        }
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

        if(_owner.IsProcessFinished == true && _lastProcessFinished == false)
        {
            StopAnimation();
            _lastProcessFinished = true;
        }
        else if(_owner.IsProcessStarted == false && _lastProcessStarted == true)
        {
            StopAnimation();
            _lastProcessStarted = false;
        }
        else if (_owner.IsProcessStarted == true && _lastProcessStarted == false)
        {
            StartAnimation();
            _lastProcessStarted = true;
        } 
        else if (_owner.IsProcessFinished == false && _lastProcessFinished == true)
        {
            _lastProcessStarted = false;
            _lastProcessFinished = false;
            _lastProgress = 0f;
        }
        else if (_owner.IsProcessStarted == true && _lastProgress < _owner.CurrentProgress)
        {
            StartAnimation();
            _lastProgress = _owner.CurrentProgress;
        }

    }

    public void RefreshModels()
    {
        foreach (ModelOnTID ingredient in _ingredientObjectList)
        {
            ingredient.Model.SetActive(false);
        }
        if (_baseObject != null)
        {
            if (_owner.InputTIDList.Count > 0)
            {
                _baseObject.SetActive(true);
            }
            else
            {
                _baseObject.SetActive(false);
            }
        }
        if (_owner.IsProcessFinished && _owner.LeftOutputAmount > 0 && _outputObject.TypeObject != null)
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
            if(_baseObject != null)
            {
                _baseObject.SetActive(false);
            }
        }
        else
        {
            if(_outputObject.TypeObject != null)
            {
                _outputObject?.TypeObject.SetActive(false);
            }
            
            foreach (int tid in _owner.InputTIDList)
            {
                if (_ingredientObjectDic.TryGetValue(tid, out GameObject ingredientObject))
                {
                    ingredientObject?.SetActive(true);
                }
            }
        }
    }

    public void StartAnimation()
    {
        foreach (DOTweenAnimation anime in _dotweenLoopAnimationList)
        {
            if (anime.tween == null)
            {
                anime.CreateTween();
            }
            if (!anime.tween.IsPlaying())
            {
                anime.tween.Restart();
            }
        }
        foreach (DOTweenAnimation anime in _dotweenOnceAnimationList)
        {
            if (anime.tween == null)
            {
                anime.CreateTween();
            }
            anime.tween.Restart();
            
        }
        foreach (ParticleSystem particle in _loopParticleList)
        {
            if (particle.isStopped)
            {
                particle.Play();
            }
        }
        foreach (EVFXType particle in _clickParticleList)
        {
            VFXFactory.Instance.CreateObject(particle, _owner.transform.position, Quaternion.identity);
        }
    }

    public void StopAnimation()
    {
        foreach (DOTweenAnimation anime in _dotweenLoopAnimationList)
        {
            anime.DOKill();
        }
        foreach (ParticleSystem particle in _loopParticleList)
        {
            if (particle.isPlaying)
            {
                particle.Stop();
            }
        }
        if(_lastProcessFinished == false && _owner.IsProcessFinished)
        {
            foreach (ParticleSystem particle in _doneParticleList)
            {
                if (particle != null && particle.isStopped)
                {
                    particle.Play();
                }
            }
        }
    }

}
