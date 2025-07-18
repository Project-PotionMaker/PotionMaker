using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftItemManager : MonoBehaviourSingleton<CraftItemManager>
{
    // 어드레서블에서 로드해올 예정
    [SerializeField]
    private GameObject _failureOutput;

    private Dictionary<string, int> _outputDataTIDDict;
    private Dictionary<string, int> _potionDataTIDDict;

    private RecipeCodeHandler _recipeCodeHandler;
    private RecipeCodeVerifier _recipeCodeVerifier;

    private PhotonView _photonView;

    public Action<GameObject> OnOutputCreated;
    public Action<GameObject> OnPotionCreated;

    protected override void Awake()
    {
        base.Awake();
        InitCraftItemManager();
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
            _potionDataTIDDict.Add(potionData.RecipeCode, potionData.TID);
        }

        _recipeCodeHandler = new RecipeCodeHandler();
        _recipeCodeVerifier = new RecipeCodeVerifier(potionDataList);
        _photonView = GetComponent<PhotonView>();
    }

    public GameObject TryCreateIngredientItem(int TID, Vector3 machinePosition)
    {
        GameObject ingredient = CraftItemFactory.Instance.Create(EInputType.Ingredient, machinePosition, Quaternion.identity);
        ingredient.GetComponent<IngredientItem>().InitIngredientData(TID);
        return ingredient;
    }

    public GameObject TryCreateOutputItem(int[] TIDList, int machineTID, EInputType type, Vector3 machinePosition)
    {
        string recipeCode = _recipeCodeHandler.MakeNewRecipeCode(TIDList, machineTID);
        GameObject output = null;
        if (_recipeCodeVerifier.IsValidProcess(recipeCode))
        {
            output = CraftItemFactory.Instance.Create(type, machinePosition, Quaternion.identity);
            output.GetComponent<OutputItem>().InitOutputData(type, _outputDataTIDDict[recipeCode]);
            return output;
        }
        return CreateFailureItem(machinePosition);
    }


    public GameObject TryCreatePotionItem(int[] TIDList, int bottlerTID, Vector3 machinePosition)
    {

        string recipeCode = _recipeCodeHandler.MakeNewRecipeCode(TIDList, bottlerTID);
        if (_recipeCodeVerifier.IsValidPotion(recipeCode))
        {
            GameObject potion = CraftItemFactory.Instance.Create(EInputType.Potion, machinePosition, Quaternion.identity);
            potion.GetComponent<Potion>().InitPotionData(_potionDataTIDDict[recipeCode]);
            return potion;
        }
        return CreateFailureItem(machinePosition);
    }

    private GameObject CreateFailureItem(Vector3 machinePosition)
    {
        return CraftItemFactory.Instance.Create(EInputType.FailureOutput, machinePosition, Quaternion.identity);
    }
}
