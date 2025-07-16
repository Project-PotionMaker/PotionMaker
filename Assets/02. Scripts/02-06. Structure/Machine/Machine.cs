using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Machine : MonoBehaviour
{
    private MachineData _data;
    public MachineData Data => _data;

    protected float _currentProgress;
    public float CurrentProgress => _currentProgress;
    protected int _leftOutputAmount;
    public float LeftOutputAmount => _leftOutputAmount;
    protected bool _isProcessFinished;
    protected bool _isProcessStarted;

    protected List<int> InputTIDList;

    protected IMachineInteractable _interactComponent;
    protected IMachineItemContainer _containerComponent;

    public virtual void Init(MachineData data, IMachineInteractable interactableComponent, IMachineItemContainer containerCompnent)
    {
        _data = data;
        InputTIDList = new List<int>();
        _interactComponent = interactableComponent;
        _containerComponent = containerCompnent;

        ClearMachine();
    }

    public virtual void ClearMachine()
    {
        InputTIDList.Clear();
        _leftOutputAmount = _data.OutputAmount;
        _isProcessFinished = false;
        _isProcessStarted = false;
        _currentProgress = 0f;
    }

    // 절구나 분쇄기를 제외하면 재료들은 아예 못들어가기도 하고, 다른 상황도 생길 수 있으므로 각자 처리 필요
    public bool TryInput(int tid, EInputType inputType)
    {
        return _containerComponent.TryInput(tid, inputType);
    }

    public bool TryInteract()
    {
        return _interactComponent.TryInteract(this);
    }

    public GameObject TakeOutput()
    {
        return _containerComponent.TakeOutput(this);
    }
}
