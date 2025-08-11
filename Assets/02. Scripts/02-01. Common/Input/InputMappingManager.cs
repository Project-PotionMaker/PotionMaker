using Steamworks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMappingManager : MonoBehaviourSingleton<InputMappingManager>
{
    [SerializeField]
    private InputActionAsset _inputActionAsset;
    public InputActionAsset InputActions => _inputActionAsset;
    
    [SerializeField]
    private GameObject _rebindOverlay;

    public event Action<InputAction, EBindingType> OnRebindComplete;
    public event Action OnRebindCanceled;
    public event Action<InputAction, EBindingType> OnRebindStarted;

    private InputActionRebindingExtensions.RebindingOperation _currentRebindOperation;

    private void Start()
    {
        // Load();
    }

    public void StartRebinding(InputAction action, EBindingType bindingType, bool excludeKeyboard = false, bool excludeGamepad = false)
    {
        if (action == null)
        {
            Debug.LogError($"Action {action.name}을 못 찾았습니다.");
            return;
        }
        

        if (_rebindOverlay != null)
        {
            _rebindOverlay.SetActive(true);
        }

        _currentRebindOperation?.Dispose();

        _inputActionAsset.Disable();

        OnRebindStarted?.Invoke(action, bindingType);

        _currentRebindOperation = action.PerformInteractiveRebinding((int)bindingType);

        if (excludeKeyboard)
        {
            _currentRebindOperation = _currentRebindOperation.WithControlsExcluding("<Keyboard>");
        }
        if (excludeGamepad)
        {
            _currentRebindOperation = _currentRebindOperation.WithControlsExcluding("<Gamepad>");
        }

        _currentRebindOperation = _currentRebindOperation.WithCancelingThrough("<Keyboard>/escape");

        _currentRebindOperation
            .OnComplete(operation => RebindComplete(operation, bindingType))
            .OnCancel(RebindCancel)
            .Start();
    }

    private void RebindComplete(InputActionRebindingExtensions.RebindingOperation operation, EBindingType bindingType)
    {
        InputAction action = operation.action;
        if (CheckDuplicateBindings(action, bindingType, operation.selectedControl.path))
        {
            // 중복이 발견되면 원래 바인딩으로 되돌리고 다시 시도
            RevertToPreviousBinding(action, bindingType);
            operation.Dispose();

            StartRebinding(action, bindingType);
            return;
        }

        _inputActionAsset.Enable();
        OnRebindComplete?.Invoke(action, bindingType);
        SaveBindingOverrides();

        operation.Dispose();
        _currentRebindOperation = null;
    }

    private void RebindCancel(InputActionRebindingExtensions.RebindingOperation operation)
    {
        _inputActionAsset.Enable();
        OnRebindCanceled?.Invoke();

        operation.Dispose();
        _currentRebindOperation = null;
    }

    private bool CheckDuplicateBindings(InputAction action, EBindingType bindingType, string newPath)
    {
        int bindingIndex = (int)bindingType;
        foreach (InputAction otherAction in _inputActionAsset)
        {
            for (int i = 0; i < otherAction.bindings.Count; i++)
            {
                if (otherAction.id == action.id && i == bindingIndex)
                {
                    continue;
                }

                if (otherAction.bindings[i].effectivePath == newPath)
                {
                    Debug.Log($"중복된 키 Binding : {newPath}는 이미 {otherAction.name}에 사용되고 있습니다.");
                    return true;
                }
            }
        }

        return false;
    }

    private void RevertToPreviousBinding(InputAction action, EBindingType bindingType)
    {
        int bindingIndex = (int)bindingType;
        InputBinding binding = action.bindings[bindingIndex];
        if (binding.hasOverrides)
        {
            // 오버라이드된 바인딩이 있으면 되돌리기
            action.ApplyBindingOverride(bindingIndex, binding.overridePath);
        }
        else
        {
            // 기본 바인딩으로
            action.RemoveBindingOverride(bindingIndex);
        }
    }

    public void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = _inputActionAsset.FindAction(actionName);
        if (action == null)
        {
            return;
        }

        action.RemoveBindingOverride(bindingIndex);
        SaveBindingOverrides();
    }

    public void ResetAllBinding()
    {
        foreach(var actionMap in _inputActionAsset.actionMaps)
        {
            actionMap.RemoveAllBindingOverrides();
        }
        SaveBindingOverrides();
    }

    public InputAction GetAction(string actionName)
    {
        return _inputActionAsset?.FindAction(actionName);
    }

    private void SaveBindingOverrides()
    {
        if (_inputActionAsset == null)
        {
            return;
        }

        string rebinds = _inputActionAsset.SaveBindingOverridesAsJson();

    }

    private void LoadBindingOverrides()
    {
        if (_inputActionAsset == null)
        {
            return;
        }

        //string rebinds = PlayerPrefs.GetString("InputBindingOverrides", string.Empty);
        //if (!string.IsNullOrEmpty(rebinds))
        //{
        //    _inputActionAsset.LoadBindingOverridesFromJson(rebinds);
        //}
    }
}
