using UnityEngine;

public class StorageStat
{
    private StorageData _data;
    public StorageData Data => _data;
    private int _ingredientTID;
    public int IngredientTID => _ingredientTID;

    public StorageStat(StorageData data, int ingredientTID)
    {
        _data = data;
        _ingredientTID = ingredientTID;
    }

    [SerializeField]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; set => _currentRotation = value; }
}