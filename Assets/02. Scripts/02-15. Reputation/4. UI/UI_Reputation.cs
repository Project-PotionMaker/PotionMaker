using UnityEngine;
using UnityEngine.UI;

public class UI_Reputation : MonoBehaviour
{
    [SerializeField]
    private Slider[] _reputationStar;
    private void Start()
    {
        ReputationManager.Instance.OnDataChanged += RefreshReputation;
        RefreshReputation();
    }

    private void RefreshReputation() // 슬라이더는 1이 빈칸이고 0이 꽉찬칸임
    {
        for (int i = 0; i < _reputationStar.Length; i++)
        {
            _reputationStar[i].value = 1.0f; // 초기화
        }

        float value = ReputationManager.Instance.Reputation.Value;
        int count = 0;
        while (value > 1.0f)
        {
            _reputationStar[count].value = 0.0f;
            value -= 1.0f;
            count++;
        }
        if (value > 0.0f && count < _reputationStar.Length)
        {
            _reputationStar[count].value = 1.0f - value; // 마지막 별은 남은 비율로 채움
        }
    }
}
