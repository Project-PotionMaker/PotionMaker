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
    private EBindingType _bindingtype;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI _actionNameText;
    [SerializeField]
    private Button _rebindButton;
    [SerializeField]
    private TextMeshProUGUI _bindKey;

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

        _actionNameText.text = _action.action.name;

        UpdateBindingDisplay();
    }

    public void StartRebind()
    {
        _rebindButton.interactable = false;
        _bindKey.text = "Press any key...";

        InputMappingManager.Instance.OnRebindComplete += HandleRebindComplete;
        InputMappingManager.Instance.OnRebindCanceled += HandleRebindCanceled;

        InputMappingManager.Instance.StartRebinding(_action, _bindingtype, false, true);
    }

    private void HandleRebindComplete(InputAction action, EBindingType bindingIndex)
    {
        if (action.name == _action.action.name && bindingIndex == _bindingtype)
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

        _bindKey.text = _action.action.GetBindingDisplayString((int)_bindingtype, options);
    }

    private void CleanUp()
    {
        InputMappingManager.Instance.OnRebindComplete -= HandleRebindComplete;
        InputMappingManager.Instance.OnRebindCanceled -= HandleRebindCanceled;

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
