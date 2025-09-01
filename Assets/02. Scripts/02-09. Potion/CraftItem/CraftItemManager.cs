using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftItemManager : NetworkBehaviourSingleton<CraftItemManager>
{
    public Action<GameObject> OnOutputCreated;
    public Action<GameObject> OnPotionCreated;

    // 어드레서블에서 로드해올 예정
    [SerializeField]
    private GameObject _failureOutput;

    private SyncDictionary<string, int> _outputDataTIDDict = new SyncDictionary<string, int>();
    private SyncDictionary<string, int> _potionDataTIDDict = new SyncDictionary<string, int>();

    private RecipeCodeHandler _recipeCodeHandler;
    private RecipeCodeVerifier _recipeCodeVerifier;

    private const int FailureOutputTID = 10000;

    public override void OnStartServer()
    {
        base.OnStartServer();
        InitCraftItemManager();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _recipeCodeHandler = new RecipeCodeHandler();
        _recipeCodeVerifier = new RecipeCodeVerifier(DataTable.Instance.GetPotionDataList());
    }

    [Server]
    private void InitCraftItemManager()
    {
        //_outputDataTIDDict = new SyncDictionary<string, int>();
        var outputDataList = DataTable.Instance.GetOutputDataList();
        foreach (var outputData in outputDataList)
        {
            _outputDataTIDDict.Add(outputData.RecipeCode, outputData.TID);
        }

        //_potionDataTIDDict = new SyncDictionary<string, int>();
        var potionDataList = DataTable.Instance.GetPotionDataList();
        foreach (var potionData in potionDataList)
        {
            _potionDataTIDDict.TryAdd(potionData.RecipeCode, potionData.TID);
        }

        _recipeCodeHandler = new RecipeCodeHandler();
        _recipeCodeVerifier = new RecipeCodeVerifier(potionDataList);
    }

    [Server]
    public GameObject TryCreateIngredientItem(int TID, Vector3 machinePosition)
    {
        GameObject ingredient = CraftItemFactory.Instance.CreateObject(EInputType.Ingredient, machinePosition, Quaternion.identity);
        ingredient.GetComponent<IngredientItem>().ServerUpdateIngredientData(TID);
        return ingredient;
    }

    [Server]
    public GameObject TryCreateOutputItem(int[] TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        string recipeCode;
        GameObject output = null;
        if (type != EInputType.Ingredient)
        {
            recipeCode = _recipeCodeHandler.MakeNewRecipeCode(TIDList, machineTID);
            if (_recipeCodeVerifier.IsValidProcess(recipeCode))
            {
                output = CraftItemFactory.Instance.CreateObject(EInputType.Output, machinePosition, Quaternion.identity);
                output.GetComponent<OutputItem>().ServerUpdateOutputData(EInputType.Output, _outputDataTIDDict[recipeCode]);
                return output;
            }
        }
        else
        {
            recipeCode = DataTable.Instance.GetIngredientData(TIDList[0]).RecipeCode;
            output = CraftItemFactory.Instance.CreateObject(EInputType.Output, machinePosition, Quaternion.identity);
            output.GetComponent<OutputItem>().ServerUpdateOutputData(EInputType.Output, _outputDataTIDDict[recipeCode]);
            return output;
        }

        return CreateFailureItem(machinePosition);
    }

    [Server]
    public GameObject TryCreatePotionItem(int[] TIDList, int bottlerTID, Vector3 machinePosition)
    {

        string recipeCode = _recipeCodeHandler.MakeNewRecipeCode(TIDList, bottlerTID);
        if (_recipeCodeVerifier.IsValidPotion(recipeCode))
        {
            GameObject potion = CraftItemFactory.Instance.CreateObject(EInputType.Potion, machinePosition, Quaternion.identity);
            potion.GetComponent<PotionItem>().ServerUpdatePotionData(_potionDataTIDDict[recipeCode]);
            return potion;
        }
        return CreateFailureItem(machinePosition);
    }

    [Server]
    private GameObject CreateFailureItem(Vector3 machinePosition)
    {
        GameObject output =CraftItemFactory.Instance.CreateObject(EInputType.Output, machinePosition, Quaternion.identity);
        output.GetComponent<OutputItem>().ServerUpdateOutputData(EInputType.FailureOutput, 10000);

        return output;
    }

    public int GetOutputTID(int[] TIDList, int machineTID, EInputType type)
    {
        string recipeCode;
        if (type != EInputType.Ingredient)
        {
            recipeCode = _recipeCodeHandler.MakeNewRecipeCode(TIDList, machineTID);
            if (_recipeCodeVerifier.IsValidProcess(recipeCode))
            {
                if(_outputDataTIDDict.TryGetValue(recipeCode, out int outputTID))
                {
                    return outputTID;
                }
                else if(_potionDataTIDDict.TryGetValue(recipeCode, out int potionTID))
                {
                    return potionTID;
                }
                else
                {
                    return FailureOutputTID;
                }
            }
            else
            {
                return FailureOutputTID;
            }
        }
        else
        {
            recipeCode = DataTable.Instance.GetIngredientData(TIDList[0]).RecipeCode;
            return _outputDataTIDDict[recipeCode];
        }
    }
}
