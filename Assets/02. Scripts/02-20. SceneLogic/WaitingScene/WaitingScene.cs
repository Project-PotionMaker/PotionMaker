using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _day;
    [SerializeField]
    private TextMeshProUGUI _money;
    [SerializeField]
    private TextMeshProUGUI _nextRentDay;
    [SerializeField]
    private TextMeshProUGUI _nextRentMoney;
    [SerializeField]
    private Image _layoutImage;

    private string _roomCode;

    public void OnClickExitButton()
    {
        MirrorNetworkManager.Instance.ServerChangeScene(MirrorNetworkManager.Instance.offlineScene);
    }

    public void Start()
    {
        StartCoroutine(Refresh_Coroutine());
    }

    private IEnumerator Refresh_Coroutine()
    {
        while(ShopInfoManager.Instance == null)
        {
            yield return new WaitForSeconds(0.05f);
        }
        while(ShopInfoManager.Instance.ShopInfo == null)
        {
            yield return new WaitForSeconds(0.05f);
        }

        ShopInfo info = ShopInfoManager.Instance.ShopInfo;
        _day.text = $"{info.Day.ToString()} 일";
        _money.text = $"{info.Currency.Value.ToString("N0")} $";
        _nextRentDay.text = $"다음 방세 지불일까지 D-{info.Rent.RentPeriod - info.Rent.RentDayCounter}";
        _nextRentMoney.text = $"지불 예정 방세 : {info.Rent.CurrentRentCost.ToString("N0")} $";
    }
}
