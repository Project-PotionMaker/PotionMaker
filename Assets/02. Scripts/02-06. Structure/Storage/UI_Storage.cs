using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    [SerializeField]
    private Storage _storage;
    [SerializeField]
    private TextMeshProUGUI _nameTextUI;

    private void Awake()
    {
        _storage.OnDataChanged += InitUIStorage;
    }

    private void InitUIStorage(StorageStat stat)
    {
        _nameTextUI.text = DataTable.Instance.GetIngredientData(stat.IngredientTID).Name;
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
