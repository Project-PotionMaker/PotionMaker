using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public abstract class Machine : MonoBehaviour, IItemContainer, IInteractable
{
    private MachineData _data;
    public MachineData Data => _data;

    protected float _currentProgress;
    protected int _leftOutputAmount;
    protected bool _isProcessFinished;
    protected bool _isProcessStarted;

    protected List<int> InputTIDList;

    [Foldout("Project")]
    [SerializeField]
    protected List<Mesh> _meshList;
    [SerializeField]
    protected MeshFilter _meshFilter;

    public virtual void Init(MachineData data)
    {
        _data = data;
        InputTIDList = new List<int>();

        ClearMachine();
    }

    public virtual void ClearMachine()
    {
        ChangeMesh(0);

        InputTIDList.Clear();
        _leftOutputAmount = _data.OutputAmount;
        _isProcessFinished = false;
        _isProcessStarted = false;
        _currentProgress = 0f;
    }

    public virtual void ChangeMesh(int index)
    {
        _meshFilter.mesh = _meshList[index];
    }

    // 절구나 분쇄기를 제외하면 재료들은 아예 못들어가기도 하고, 다른 상황도 생길 수 있으므로 각자 처리 필요
    public abstract bool TryInput(int tid, EInputType inputType);

    public virtual bool CanInteract()
    {
        if(InputTIDList.Count == _data.MaxInputCount && _isProcessFinished == false)
        {
            return true;
        }
        return false;
    }

    public abstract bool TryInteract();

    public abstract GameObject TakeOutput();
}
