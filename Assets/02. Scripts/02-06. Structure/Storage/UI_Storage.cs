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

    private void InitUIStorage()
    {
        //_nameTextUI.text = DataTable.Instance.GetIngredientData(_storage.IngredientTID).Name;
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
