using TMPro;
using UnityEngine;

public class UI_LoadingScene : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private LoadingScene _loadingScene;

    [SerializeField]
    private TextMeshProUGUI _textProgressPercentage;

    [SerializeField]
    private TextMeshProUGUI _textTip;

    private void Start()
    {
        _loadingScene.OnTipChanged += RefreshTip;
        Global.Instance.OnDataLoaded += () =>
        {
            RefreshTip(DataTable.Instance.GetTipData(10000).Description);
        };
    }

    public void RefreshProgressPercentage(float progress)
    {
        if (1f <= progress)
        {
            _textProgressPercentage.text = $"{(int)progress}%";
        }
        else
        {
            _textProgressPercentage.text = $"{(int)(progress * 100)}%";
        }
    }

    public void RefreshTip(string tip)
    {
        _textTip.text = tip;
    }
}
