
using System.Collections.Generic;
using UnityEngine;

public class OutputManager : MonoBehaviourSingleton<OutputManager>
{
    // 어드레서블에서 로드해올 예정
    [SerializeField]
    private GameObject _failureOutput;

    private Dictionary<string, int> _outputDataTIDDict;
    private Dictionary<string, int> _potionDataTIDDict;

    private RecipeCodeHandler _recipeCodeHandler;
    private RecipeCodeVerifier _recipeCodeVerifier;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _outputDataTIDDict = new Dictionary<string, int>();
        var outputDataList = DataTable.Instance.GetOutputDataList();
        foreach (var outputData in outputDataList)
        {
            _outputDataTIDDict.Add(outputData.RecipeCode, outputData.TID);
        }

        _potionDataTIDDict = new Dictionary<string, int>();
        var potionDataList = DataTable.Instance.GetPotionDataList();
        foreach (var potionData in potionDataList)
        {
            _potionDataTIDDict.Add(potionData.RecipeCode, potionData.TID);
        }

        _recipeCodeHandler = new RecipeCodeHandler();
        _recipeCodeVerifier = new RecipeCodeVerifier(potionDataList);
    }

    public GameObject TryCreateOutput(List<int> TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        string input1RecipeCode = DataTable.Instance.GetOutputData(TIDList[0]).RecipeCode, input2RecipeCode = null;
        string recipeCode;
        if (type == EInputType.HeatingPotOutput)
        {
            input2RecipeCode = DataTable.Instance.GetOutputData(TIDList[1]).RecipeCode;
            string recipeCodeNumberPart = _recipeCodeHandler.GenerateNumberPartCode(input1RecipeCode, input2RecipeCode);
            recipeCode = _recipeCodeHandler.AddMachineCode(recipeCodeNumberPart, machineTID);
        }
        else
        {
            recipeCode = _recipeCodeHandler.AddMachineCode(input1RecipeCode, machineTID);
        }

        if (_recipeCodeVerifier.IsValidProcess(recipeCode))
        {
            GameObject output = OutputFactory.Instance.Create(type, machinePosition, Quaternion.identity);
            output.GetComponent<Output>().InitOutputData(type, _outputDataTIDDict[recipeCode]);
            return output;
        }
        return CreateFailureOutput(machinePosition);
    }

    public GameObject TryCreatePotion(List<int> TIDList, int bottlerTID, Vector3 machinePosition)
    {
        string recipeCode = 
            _recipeCodeHandler.AddMachineCode(DataTable.Instance.GetOutputData(TIDList[0]).RecipeCode, bottlerTID);

        if (_recipeCodeVerifier.IsValidPotion(recipeCode))
        {
            GameObject potion = OutputFactory.Instance.Create(EInputType.Potion, machinePosition, Quaternion.identity);
            potion.GetComponent<Potion>().InitPotionData(_potionDataTIDDict[recipeCode]);
            return potion;
        }
        return CreateFailureOutput(machinePosition);
    }

    private GameObject CreateFailureOutput(Vector3 machinePosition)
    {
        return OutputFactory.Instance.Create(EInputType.FailureOutput, machinePosition, Quaternion.identity);
    }
}
