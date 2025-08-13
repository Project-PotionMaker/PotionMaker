using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_RebindAction : MonoBehaviour
{
    [Header("Binding")]
    [SerializeField]
    private InputActionReference _action;
    [SerializeField]
    private EBindingType _bindingType;
    public EBindingType BindingType { get => _bindingType; set => _bindingType = value; }

    [Header("UI")]
    [SerializeField]
    private Button _rebindButton;
    [SerializeField]
    private TextMeshProUGUI _bindKeyText;
    [SerializeField]
    private Image _bindImage;
    [SerializeField]
    private GameObject _activeOverlay;

    private void Start()
    {
        InputMappingManager.Instance.OnBindingReset += UpdateBindingDisplay;
        Initialize();
    }

    private void OnDestroy()
    {
        if (InputMappingManager.Instance != null)
        {
            InputMappingManager.Instance.OnRebindComplete -= HandleRebindComplete;
            InputMappingManager.Instance.OnRebindCanceled -= HandleRebindCanceled;
            InputMappingManager.Instance.OnBindingReset -= UpdateBindingDisplay;
        }
    }

    public void Initialize()
    {
        if (_action == null)
        {
            return;
        }

        if (_activeOverlay != null)
        {
            _activeOverlay.SetActive(false);
        }

        UpdateBindingDisplay();
    }

    public void StartRebind()
    {
        _rebindButton.interactable = false;
        _activeOverlay.SetActive(true);

        InputMappingManager.Instance.OnRebindComplete += HandleRebindComplete;
        InputMappingManager.Instance.OnRebindCanceled += HandleRebindCanceled;

        InputMappingManager.Instance.StartRebinding(_action, _bindingType);
    }

    private void HandleRebindComplete(InputAction action, EBindingType bindingType)
    {
        if (action.name == _action.action.name && bindingType == _bindingType)
        {
            UpdateBindingDisplay();
            CleanUp();
        }
    }

    private void HandleRebindCanceled()
    {
        UpdateBindingDisplay();
        CleanUp();
    }

    private void UpdateBindingDisplay()
    {
        if (_action == null || _action.action == null)
        {
            _bindKeyText.text = "N/A";
            _bindKeyText.gameObject.SetActive(true);
            _bindImage.gameObject.SetActive(false);
            return;
        }

        

        int bindingIndex = (int)_bindingType;
        string path = _action.action.bindings[bindingIndex].effectivePath;

        Sprite icon = BindingIconManager.Instance.GetSpriteForPath(path);

        if (icon != null)
        {
            _bindImage.sprite = icon;
            _bindImage.gameObject.SetActive(true);
            _bindKeyText.gameObject.SetActive(false);
        }
        else
        {
            var options = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.DontIncludeInteractions;
            _bindKeyText.text = _action.action.GetBindingDisplayString((int)_bindingType, options);
            _bindKeyText.gameObject.SetActive(true);
            _bindImage.gameObject.SetActive(false);
        }

            
    }

    private void CleanUp()
    {
        InputMappingManager.Instance.OnRebindComplete -= HandleRebindComplete;
        InputMappingManager.Instance.OnRebindCanceled -= HandleRebindCanceled;

        _activeOverlay.SetActive(false);
        _rebindButton.interactable = true;
    }
}
