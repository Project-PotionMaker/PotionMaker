using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UI_Customer : MonoBehaviour
{
    [Foldout("UI컴포넌트")]
    [SerializeField]
    private Slider _enduranceSlider;
    [SerializeField]
    private Image _stateImage;

    [Foldout("State 아이콘 이미지")]
    [SerializeField]
    private Sprite _lingingIcon;
    [SerializeField]
    private Sprite _waitingIcon;

    private Customer _owner;

    private void Start()
    {
        _owner = GetComponentInParent<Customer>();
        _enduranceSlider.maxValue = 1f;

        _owner.OnStateChanged += SetSlide; // 상태 변경 이벤트에 슬라이더 설정 메서드 등록
        _owner.OnStateChanged += SetStateImage; // 상태 변경 이벤트에 상태 텍스트 설정 메서드 등록
        _owner.CustomerEndurance.OnEnduranceChanged += SetSlide; // 인내심 변경 이벤트에 슬라이더 설정 메서드 등록
        SetSlide(); // 초기 슬라이더 설정
        SetStateImage(); // 초기 상태 텍스트 설정
    }

    private void SetSlide()
    {
        if(_owner.CurrentState == ECustomerStateType.Leaving || _owner.CurrentState == ECustomerStateType.PickingUp)
        {
            _enduranceSlider.gameObject.SetActive(false); // Leaving 또는 PickingUp 상태에서는 슬라이더 숨김
            return;
        }
        else if (_owner.CustomerEndurance != null)
        {
            _enduranceSlider.gameObject.SetActive(true); // 다른 상태에서는 슬라이더 표시
            _enduranceSlider.value = _owner.CustomerEndurance.EnduranceRate;
        }
    }
    private void SetStateImage()
    {
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _stateImage.gameObject.SetActive(true); 
            _stateImage.sprite = _lingingIcon;
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            _stateImage.gameObject.SetActive(false); 
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            _stateImage.gameObject.SetActive(false);
        }
        //else if (_owner.CurrentState == ECustomerStateType.Waiting)
        //{
        //    _stateImage.gameObject.SetActive(true); //TODO : 어떤 포션인지 아이콘으로 표현
        //    _stateImage.sprite = _waitingIcon;
        //}
    }


}
