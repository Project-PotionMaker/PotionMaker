using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeatingPotAnimation : MonoBehaviour, IMachineAnimation
{
    [SerializeField]
    private List<DOTweenAnimation> _dotweenAnimationList;

    [SerializeField]
    private List<ParticleSystem> _particleList;

    [SerializeField]
    private ColorOnType _outputObject;
    [SerializeField]
    private GameObject _baseObject;
    private MaterialPropertyBlock _mpb;

    private Machine _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<Machine>();
        _outputObject?.TypeObject.SetActive(false);
        _mpb = new MaterialPropertyBlock();
        foreach (var particle in _particleList)
        {
            if (particle != null)
            {
                particle.Stop();
            }
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

        if (_owner.IsProcessStarted)
        {
            StartAnimation();
        }
        else
        {
            EndAnimation();
        }
    }

    public void RefreshModels()
    {
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
            if(_owner.LeftOutputAmount > 0 || _owner.InputTIDList.Count > 0)
            {
                _baseObject.SetActive(true);
            }
            else
            {
                _baseObject.SetActive(false);
            }
        }
    }

    public void EndAnimation()
    {
        foreach (DOTweenAnimation anime in _dotweenAnimationList)
        {
            anime.DOKill();
        }
        foreach (ParticleSystem particle in _particleList)
        {
            if (particle.isPlaying)
            {
                particle.Stop();
            }
        }
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
            if (anime.tween == null)
            {
                anime.CreateTween();
            }
            if (!anime.tween.IsPlaying())
            {
                anime.tween.Restart();
            }
        }
        foreach (ParticleSystem particle in _particleList)
        {
            if (particle.isStopped)
            {
                particle.Play();
            }
        }
    }

    public void StopAnimation()
    {
    }

}
