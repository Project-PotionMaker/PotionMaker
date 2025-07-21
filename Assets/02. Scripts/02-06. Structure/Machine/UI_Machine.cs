using TMPro;
using UnityEngine;

public class UI_Machine : MonoBehaviour
{
    // 테스트 코드들입니다.
    public Machine machine;
    public TextMeshProUGUI testText;

    void Update()
    {
        if(machine != null)
        {
            testText.text = $"{machine.GetStat().CurrentProgress}";
        }
    }
}
