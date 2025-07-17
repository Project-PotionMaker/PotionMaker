using UnityEngine;
using UnityEngine.UI;   

public class UI_Endurance : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private Customer _owner;

    private void Start()
    {
        _owner = GetComponentInParent<Customer>();
        slider.maxValue = 1f;
    }

    private void Update()
    {
        if(_owner.CurrentState == ECustomerStateType.Leaving || _owner.CurrentState == ECustomerStateType.PickingUp)
        {
            slider.gameObject.SetActive(false); // Leaving 또는 PickingUp 상태에서는 슬라이더 숨김
            return;
        }
        else if (_owner.CustomerEndurance != null)
        {
            slider.gameObject.SetActive(true); // 다른 상태에서는 슬라이더 표시
            slider.value = _owner.CustomerEndurance.EnduranceRate;
        }
    }

}
