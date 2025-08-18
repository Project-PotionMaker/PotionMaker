using Unity.VisualScripting;
using UnityEngine;

public class MachineInputContainer : IInputContainer<Machine>
{
    public bool ServerTryInput(Machine machine, int tid, EInputType inputType, GameObject inputObject)
    {
        switch (inputType)
        {
            case EInputType.Ingredient:
                IngredientData ingredientData = DataTable.Instance.GetIngredientData(tid);
                if(ingredientData.AvailableMachineTID != machine.Data.TID)
                {
                    return false;
                }
                break;
            case EInputType.Output:
                OutputData outputData = DataTable.Instance.GetOutputData(tid);
                if(outputData.AvailableMachineTIDList.Contains(machine.Data.TID) == false)
                {
                    return false;
                }
                break;
            case EInputType.Potion:
                return false;
            default:
                return false;
        }

        if (machine.InputTIDList.Count + 1 > machine.Data.MaxInputCount ||
            machine.IsProcessFinished ||
            machine.InputTIDList.Contains(tid))
        {
            return false;
        }

        machine.ServerSetInputType(inputType);
        machine.ServerAddInputTID(tid);

        return true;
    }
}
