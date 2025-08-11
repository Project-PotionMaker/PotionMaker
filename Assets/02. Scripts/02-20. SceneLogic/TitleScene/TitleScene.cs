using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private Image _titleImage;

    [SerializeField]
    private ParticleSystem _titleVFX;

    [SerializeField]
    private TextMeshProUGUI _pressSpaceToStartText;

    [SerializeField]
    private float _titleImageFadeTime;

    [SerializeField]
    private float _pressSpaceToStartTextFadeTime;

    private bool _isTitleFadeEnd = false;

    private void Start()
    {
        TitleSceneDirection();
    }

    private void Update()
    {
        if (_isTitleFadeEnd && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("1_LobbyScene");
        }
    }

    private void TitleSceneDirection()
    {
        _titleImage.DOFade(1f, _titleImageFadeTime).OnComplete
            (() =>
            {
                _isTitleFadeEnd = true;
                _pressSpaceToStartText.DOFade(1f, _pressSpaceToStartTextFadeTime).SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InSine);
                _titleVFX.Play();
            });
    }
}
