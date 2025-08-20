using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public enum ECustomerEmojiType
{
    Happy, // 행복
    Sleepy, // 졸림
    Angry // 화남
}
public class UI_Customer : MonoBehaviour
{
    private const string ASSET_PREFIX = "Image_Potion_";
    [Foldout("UI컴포넌트")]
    [SerializeField]
    private Slider _enduranceSlider;
    [SerializeField]
    private Image _stateImage;
    [SerializeField]
    private GameObject _buffIcon;
    [SerializeField]
    private CanvasGroup _canvas;

    [SerializeField]
    private ParticleSystem _happy;
    [SerializeField]
    private ParticleSystem _sleepy;
    [SerializeField]
    private ParticleSystem _angry;

    private bool _isSleepPlayed = false;

    [Foldout("State 아이콘 이미지")]
    [SerializeField]
    private Sprite _liningIcon;

    private Customer _owner;

    private void Start()
    {
        _owner = GetComponentInParent<Customer>();
        _enduranceSlider.maxValue = 1f;

        _owner.OnStateChanged += SetSlide;
        _owner.OnStateChanged += SetStateImage;
        _owner.CustomerEndurance.OnEnduranceChanged += SetSlide; // 인내심 변경 이벤트에 슬라이더 설정 메서드 등록
        _owner.CustomerEndurance.OnEnduranceChanged += SetBuffIcon;
        _owner.OnCreated += SetSlide; // 생성 시 슬라이더 설정 메서드 등록
        _owner.OnCreated += SetStateImage; // 생성 시 상태 이미지 설정 메서드 등록
        SetSlide(); // 초기 슬라이더 설정
        SetStateImage(); // 초기 상태 텍스트 설정
        _owner.CustomerMove.OnSuccess += ShowEmoji; // 성공 이벤트에 이모지 표시 메서드 등록
        _owner.CustomerEndurance.OnEnduranceZero += ShowEmoji; // 인내심이 0이 될 때 이모지 표시 메서드 등록
    }

    private void OnEnable()
    {
        _isSleepPlayed = false; // Enable 시 슬립 이펙트 초기화
    }

    private void SetSlide()
    {
        if(_owner.CurrentState == ECustomerStateType.Leaving || _owner.CurrentState == ECustomerStateType.PickingUp)
        {
            return;
        }
        else if (_owner.CustomerEndurance != null)
        {
            _enduranceSlider.value = _owner.CustomerEndurance.EnduranceRate;
            float rate = _owner.CustomerEndurance.EnduranceRate;
            Color targetColor = Color.white;
            if (rate > 0.7f)
            {
                _isSleepPlayed = false;
                targetColor = Color.green;
            }
            else if (rate>0.5f)
            {
                _isSleepPlayed = false;
                targetColor = Color.Lerp(Color.yellow, Color.green, 5*(rate-0.5f));
            }else if(rate > 0.3f)
            {
                if(_isSleepPlayed == false)
                {
                    ShowEmoji(ECustomerEmojiType.Sleepy); // 슬립 이펙트 재생
                    _isSleepPlayed = true; // 슬립 이펙트가 재생되었음을 표시
                }
                targetColor = Color.Lerp(Color.red, Color.yellow, 5 * (rate-0.3f));
            }
            else if(rate>0f)
            {
                targetColor = Color.red;
            }

                // 슬라이더 Fill 이미지의 색 변경
                _enduranceSlider.fillRect.GetComponent<Image>().color = targetColor;
        }
    }
    private void SetStateImage()
    {
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _canvas.alpha = 1f; // 줄 서는 상태에서는 캔버스 보이기
            _stateImage.sprite = _liningIcon;
            ColorUtility.TryParseHtmlString("#FA3A3A", out Color color);
            _stateImage.color = Color.white;
            SetBuffIcon();
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
           _canvas.alpha = 0f; 
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            _canvas.alpha = 0f;
        }
        else if (_owner.CurrentState == ECustomerStateType.Sitting)
        {
            _canvas.alpha = 1f;
            _stateImage.sprite = ImageManager.Instance.GetImage<PotionData>(_owner.RequestedPotionTID);
            _stateImage.color = Color.white;
        }
    }

    public void SetBuffIcon()
    {
        if(_owner.CustomerEndurance.LoseEnduranceSpeed < 1f)
        {
            _buffIcon.SetActive(true);
        }
        else
        {
            _buffIcon.SetActive(false);
        }
    }

    private void ShowEmoji(ECustomerEmojiType emoji)
    {
        if(emoji == ECustomerEmojiType.Happy)
        {
            _happy.Play();
        }
        else if (emoji == ECustomerEmojiType.Sleepy)
        {
            _sleepy.Play();
        }
        else if (emoji == ECustomerEmojiType.Angry)
        {
            _angry.Play();
        }
    }


}
