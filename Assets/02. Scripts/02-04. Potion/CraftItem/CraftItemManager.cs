//using Photon.Pun;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftItemManager : NetworkBehaviourSingleton<CraftItemManager>
{
    // 어드레서블에서 로드해올 예정
    [SerializeField]
    private GameObject _failureOutput;

    private Dictionary<string, int> _outputDataTIDDict;
    private Dictionary<string, int> _potionDataTIDDict;

    private RecipeCodeHandler _recipeCodeHandler;
    private RecipeCodeVerifier _recipeCodeVerifier;

    //private PhotonView _photonView;

    public Action<GameObject> OnOutputCreated;
    public Action<GameObject> OnPotionCreated;

    protected override void Awake()
    {
        base.Awake();
        Global.Instance.OnDataLoaded += InitCraftItemManager;
    }

    private void InitCraftItemManager()
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
            _potionDataTIDDict.TryAdd(potionData.RecipeCode, potionData.TID);
        }

        _recipeCodeHandler = new RecipeCodeHandler();
        _recipeCodeVerifier = new RecipeCodeVerifier(potionDataList);
        //_photonView = GetComponent<PhotonView>();
    }

    [Server]
    public GameObject TryCreateIngredientItem(int TID, Vector3 machinePosition)
    {
        GameObject ingredient = CraftItemFactory.Instance.CreateObject(EInputType.Ingredient, machinePosition, Quaternion.identity);
        ingredient.GetComponent<IngredientItem>().InitIngredientData(TID);
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
                output.GetComponent<OutputItem>().InitOutputData(EInputType.Output, _outputDataTIDDict[recipeCode]);
                return output;
            }
        }
        else
        {
            recipeCode = DataTable.Instance.GetIngredientData(TIDList[0]).RecipeCode;
            output = CraftItemFactory.Instance.CreateObject(EInputType.Output, machinePosition, Quaternion.identity);
            output.GetComponent<OutputItem>().InitOutputData(EInputType.Output, _outputDataTIDDict[recipeCode]);
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
            potion.GetComponent<PotionItem>().UpdatePotionData(_potionDataTIDDict[recipeCode]);
            return potion;
        }
        return CreateFailureItem(machinePosition);
    }

    [Server]
    private GameObject CreateFailureItem(Vector3 machinePosition)
    {
        GameObject output =CraftItemFactory.Instance.CreateObject(EInputType.Output, machinePosition, Quaternion.identity);
        output.GetComponent<OutputItem>().InitOutputData(EInputType.FailureOutput, 10000);

        return output;
    }
}
