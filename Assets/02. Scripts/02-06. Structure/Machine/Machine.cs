using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour, IMachineItemContainer
{
    private MachineData _data;
    public MachineData Data => _data;

    private float _currentProgress;
    public float CurrentProgress { get => _currentProgress; set => _currentProgress = value; }
    private int _leftOutputAmount;
    private bool _isProcessFinished;
    public bool IsProcessFinished { get => _isProcessFinished; set => _isProcessFinished = value; }
    private bool _isProcessStarted;
    public bool IsProcessStarted { get => _isProcessStarted; set => _isProcessStarted = value; }

    private List<int> _inputTIDList;
    public List<int> InputTIDList => _inputTIDList;

    private IMachineInteractable _interactComponent;

    public void Init(MachineData data, IMachineInteractable interactableComponent)
    {
        _data = data;
        _inputTIDList = new List<int>();
        _interactComponent = interactableComponent;

        ClearMachine();
    }

    public virtual void ClearMachine()
    {
        _inputTIDList.Clear();
        _leftOutputAmount = _data.OutputAmount;
        _isProcessFinished = false;
        _isProcessStarted = false;
        _currentProgress = 0f;
    }

    public bool TryInteract()
    {
        return _interactComponent.TryInteract(this);
    }

    public bool TryInput(int tid, EInputType inputType)
    {
        if (InputTIDList.Count + 1 > Data.MaxInputCount ||
            _isProcessFinished ||
            InputTIDList.Contains(tid))
        {
            return false;
        }

        InputTIDList.Add(tid);

        return true;
    }

    public GameObject TakeOutput()
    {
        if (_isProcessFinished)
        {
            //여기 Machine에 합쳐버리면 시트 테이블 타입 주는 곳에서 어떻게 판별할까?
            GameObject output = OutputManager.Instance.CreateOutput(InputTIDList, EInputType.Output);
            _leftOutputAmount--;
            if (_leftOutputAmount <= 0)
            {
                ClearMachine();
            }

            return output;
        }

        return null;
    }
}
