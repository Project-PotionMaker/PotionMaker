using TMPro;
using UnityEngine;

public class UI_Currency : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _coinValueTextUI;

    private void Awake()
    {
        CurrencyManager.Instance.OnDataChanged += Refresh;
    }
    public void Refresh()
    {
        _coinValueTextUI.text = CurrencyManager.Instance.Coin.Value.ToString("N0");
    }
}
