using System;
using UnityEngine;

[Serializable]
public class FurnitureStat
{
    private FurnitureData _data;
    public FurnitureData Data => _data;
    [SerializeField]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; set => _currentRotation = value; }
    [SerializeField]
    private GameObject _inputObject;
    public GameObject InputObject { get => _inputObject; set => _inputObject = value; }
    [SerializeField]
    private Transform _inputPosition;
    public Transform InputPosition { get => _inputPosition; set => _inputPosition = value; }

    public FurnitureStat(FurnitureData data)
    {
        _data = data;
    }


}
