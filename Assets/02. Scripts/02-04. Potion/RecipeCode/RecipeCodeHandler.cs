using System;
using UnityEngine;

public class RecipeCodeHandler
{
    public string MakeNewRecipeCode(int[] TIDList, int machineTID)
    {
        string outputRecipeCode;
        string input1RecipeCode
            = DataTable.Instance.GetOutputData(TIDList[0]).RecipeCode;
        string input2RecipeCode = null;
        if (TIDList.Length == 2)
        {
            input2RecipeCode = DataTable.Instance.GetOutputData(TIDList[1]).RecipeCode;
            string recipeCodeNumberPart = GenerateNumberPartCode(input1RecipeCode, input2RecipeCode);
            outputRecipeCode = AddMachineCode(recipeCodeNumberPart, machineTID);
        }
        else
        {
            outputRecipeCode = AddMachineCode(input1RecipeCode, machineTID);
        }
        return outputRecipeCode;
    }

    private string GenerateNumberPartCode(string a, string b)
    {
        if (!int.TryParse(a, out int idA))
        {
            throw new ArgumentException($"유효한 레시피코드가 아닙니다 : {a}");
        }

        if (!int.TryParse(b, out int idB))
        {
            throw new ArgumentException($"유효한 레시피코드가 아닙니다 : {b}");
        }

        if (idA > idB)
        {
            (idA, idB) = (idB, idA);
        }

        int code = idA * 100 + idB;

        return code.ToString("D4");
    }

    private string AddMachineCode(string recipeCode, int machineTID)
    {
        return recipeCode += 
            DataTable.Instance.GetMachineData(machineTID).MachineCode;
    }
}
