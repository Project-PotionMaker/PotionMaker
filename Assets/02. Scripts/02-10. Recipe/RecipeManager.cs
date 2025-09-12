using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviourSingleton<RecipeManager>
{
    private Dictionary<int, PotionData> _potionDataDict = new Dictionary<int, PotionData>();
    public Dictionary<int, PotionData> PotionDataDict => _potionDataDict;

    private Dictionary<char, MachineData> _machineDataDict = new Dictionary<char, MachineData>();
    public Dictionary<char, MachineData> MachineDataDict => _machineDataDict;

    private Dictionary<int, IngredientData> _ingredientDataDict = new Dictionary<int, IngredientData>();
    public Dictionary<int, IngredientData> IngredientDataDict => _ingredientDataDict;

    public event Action OnInitialized;

    private void Start()
    {
        InitRecipe();
    }

    private void InitRecipe()
    {
        ReadOnlyList<PotionData> potionDataList = DataTable.Instance.GetPotionDataList();

        foreach (PotionData potionData in potionDataList)
        {
            _potionDataDict[potionData.TID] = potionData;
        }

        ReadOnlyList<MachineData> machineDataList = DataTable.Instance.GetMachineDataList();

        foreach (MachineData machineData in machineDataList)
        {
            if (_machineDataDict.ContainsKey(machineData.MachineCode))
            {
                continue;
            }
            _machineDataDict[machineData.MachineCode] = machineData;
        }

        ReadOnlyList<IngredientData> ingredientDataList = DataTable.Instance.GetIngredientDataList();

        foreach (IngredientData ingredientData in ingredientDataList)
        {
            _ingredientDataDict[ingredientData.TID] = ingredientData;
        }

        OnInitialized?.Invoke();
    }
}
