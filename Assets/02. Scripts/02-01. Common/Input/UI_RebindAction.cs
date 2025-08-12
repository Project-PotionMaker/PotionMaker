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
    private TextMeshProUGUI _bindKey;
    [SerializeField]
    private GameObject _activeOverlay;

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        if (InputMappingManager.Instance != null)
        {
            InputMappingManager.Instance.OnRebindComplete -= HandleRebindComplete;
            InputMappingManager.Instance.OnRebindCanceled -= HandleRebindCanceled;
        }
    }

    public void Initialize()
    {
        if (_action == null)
        {
            return;
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
            _bindKey.text = "N/A";
            return;
        }

        var options = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.DontIncludeInteractions;

        _bindKey.text = _action.action.GetBindingDisplayString((int)_bindingType, options);
    }

    private void CleanUp()
    {
        InputMappingManager.Instance.OnRebindComplete -= HandleRebindComplete;
        InputMappingManager.Instance.OnRebindCanceled -= HandleRebindCanceled;

        _activeOverlay.SetActive(false);
        _rebindButton.interactable = true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        Initialize();
    }
#endif
}
