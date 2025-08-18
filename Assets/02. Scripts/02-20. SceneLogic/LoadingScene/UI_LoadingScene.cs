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

    private void Awake()
    {
        _loadingScene.OnTipChanged += RefreshTip;
        _loadingScene.OnProgressChanged += RefreshProgressPercentage;
    }

    private void Start()
    {
        Global.Instance.OnDataLoaded += () =>
        {
            RefreshTip(DataTable.Instance.GetTipData(10000).Description);
        };
    }
    public void RefreshTip(string tip)
    {
        _textTip.text = tip;
    }

    public void RefreshProgressPercentage(float progress)
    {
        _textProgressPercentage.text = $"{Mathf.FloorToInt(progress * 100)}%";
    }
}
