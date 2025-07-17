using System.Collections.Generic;
using UnityEngine;

public class MachineStat : MonoBehaviour
{
    private MachineData _data;
    public MachineData Data => _data;

    private float _currentProgress;
    public float CurrentProgress { get => _currentProgress; set => _currentProgress = value; }
    private int _leftOutputAmount;
    public int LeftOutputAmount { get => _leftOutputAmount; set => _leftOutputAmount = value; }
    private bool _isProcessFinished;
    public bool IsProcessFinished { get => _isProcessFinished; set => _isProcessFinished = value; }
    private bool _isProcessStarted;
    public bool IsProcessStarted { get => _isProcessStarted; set => _isProcessStarted = value; }

    private List<int> _inputTIDList;
    public List<int> InputTIDList => _inputTIDList;

    public MachineStat(MachineData data)
    {
        _data = data;
        _inputTIDList = new List<int>();
    }

    public void ClearMachine()
    {
        _inputTIDList.Clear();
        _leftOutputAmount = _data.OutputAmount;
        _isProcessFinished = false;
        _isProcessStarted = false;
        _currentProgress = 0f;
    }
}
