using System;
using UnityEngine;

public class RecipeCodeHandler
{

    public string GenerateNumberPartCode(string a, string b)
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

    public string AddMachineCode(string recipeCode, int machineTID)
    {
        return recipeCode += 
            DataTable.Instance.GetMachineData(machineTID).MachineCode;
    }
}
