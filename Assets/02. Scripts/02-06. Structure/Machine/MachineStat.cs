using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

[Serializable]
public class MachineStat
{
    private MachineData _data;
    public MachineData Data => _data;

    [SerializeField]
    private float _currentProgress;
    public float CurrentProgress 
    { 
        get => _currentProgress; 
        set
        {
            _currentProgress = Mathf.Clamp(value, 0, _data.MaxProgress);
            if (_currentProgress == _data.MaxProgress)
            {
                _isProcessFinished = true;
            }
        }
    }
    [SerializeField]
    private int _leftOutputAmount;
    public int LeftOutputAmount { get => _leftOutputAmount; set => _leftOutputAmount = value; }
    [SerializeField]
    private bool _isProcessFinished;
    public bool IsProcessFinished { get => _isProcessFinished; set => _isProcessFinished = value; }
    [SerializeField]
    private bool _isProcessStarted;
    public bool IsProcessStarted { get => _isProcessStarted; set => _isProcessStarted = value; }
    [SerializeField]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; set => _currentRotation = value; }

    [SerializeField]
    private List<int> _inputTIDList;
    public List<int> InputTIDList { get => _inputTIDList; set => _inputTIDList = value; }
    [SerializeField]
    private EInputType _inputType;
    public EInputType InputType { get => _inputType; set => _inputType = value; }

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
