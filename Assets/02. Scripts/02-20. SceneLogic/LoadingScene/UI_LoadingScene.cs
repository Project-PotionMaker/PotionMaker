using TMPro;
using UnityEngine;

public class UI_LoadingScene : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private TextMeshProUGUI _textProgressPercentage;

    [SerializeField]
    private TextMeshProUGUI _textTip;

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
