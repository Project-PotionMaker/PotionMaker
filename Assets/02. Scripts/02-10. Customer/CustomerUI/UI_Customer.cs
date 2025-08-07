using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UI_Customer : MonoBehaviour
{
    private const string ASSET_PREFIX = "Image_Potion_";
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

        _owner.OnStateChanged += SetSlide;
        _owner.OnStateChanged += SetStateImage;
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
            float rate = _owner.CustomerEndurance.EnduranceRate;
            Color targetColor = Color.white;
            if (rate>0.5f)
            {
                targetColor = Color.Lerp(Color.yellow, Color.green, 2*(rate-0.5f));
            }else if(rate <= 0.5f)
            {
                targetColor = Color.Lerp(Color.red, Color.yellow, 2 * rate);
            }

            // 슬라이더 Fill 이미지의 색 변경
            _enduranceSlider.fillRect.GetComponent<Image>().color = targetColor;
        }
    }
    async private void SetStateImage()
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
        else if (_owner.CurrentState == ECustomerStateType.Sitting)
        {
            _stateImage.gameObject.SetActive(true);
            _stateImage.sprite = await AssetManager.Instance.LoadAsset<Sprite>($"{ASSET_PREFIX}{_owner.RequestedPotionTID}");
        }
    }


}
