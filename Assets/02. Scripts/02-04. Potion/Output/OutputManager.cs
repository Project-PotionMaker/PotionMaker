
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class OutputManager : MonoBehaviourSingleton<OutputManager>
{
    [SerializeField]
    private GameObject _failureOutput;

    private Dictionary<string, int> _potionDataDict;

    private RecipeCodeHandler _recipeCodeHandler;
    private RecipeCodeVerifier _recipeCodeVerifier;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        var potionDataList = DataTable.Instance.GetPotionDataList();
        foreach (var potionData in potionDataList)
        {
            _potionDataDict.Add(potionData.RecipeCode, potionData.TID);
        }

        _recipeCodeHandler = new RecipeCodeHandler();
        _recipeCodeVerifier = new RecipeCodeVerifier(potionDataList);
    }

    public GameObject TryCreateOutput(List<int> TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        string recipeCode;
        if (type == EInputType.HeatingPotOutput)
        {
            recipeCode = _recipeCodeHandler.GenerateNumberPartCode(
                DataTable.Instance.GetOutputData(TIDList[0]).RecipeCode,
                DataTable.Instance.GetOutputData(TIDList[1]).RecipeCode);
        }
        else
        {
            recipeCode = DataTable.Instance.GetOutputData(TIDList[0]).RecipeCode;
        }

        if (_recipeCodeVerifier.IsValidProcess(recipeCode))
        {
            string newRecipeCode = _recipeCodeHandler.AddMachineCode(recipeCode, machineTID);

            return OutputFactory.Instance.Create(type, machinePosition, Quaternion.identity);
        }
        return CreateFailureOutput(machinePosition);
    }

    public GameObject TryCreatePotion(List<int> TIDList, int bottlerTID, Vector3 machinePosition)
    {
        string recipeCode = 
            _recipeCodeHandler.AddMachineCode(DataTable.Instance.GetOutputData(TIDList[0]).RecipeCode, bottlerTID);

        if (_recipeCodeVerifier.IsValidPotion(recipeCode))
        {
            string newRecipeCode = _recipeCodeHandler.AddMachineCode(recipeCode, bottlerTID);
            return OutputFactory.Instance.Create(EInputType.Potion, machinePosition, Quaternion.identity);
        }
        return CreateFailureOutput(machinePosition);
    }

    private GameObject CreateFailureOutput(Vector3 machinePosition)
    {
        return OutputFactory.Instance.Create(EInputType.FailureOutput, machinePosition, Quaternion.identity);
    }
}
