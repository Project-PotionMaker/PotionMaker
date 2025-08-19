using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class MachineInputContainer : IInputContainer<Machine>
{
    public bool ServerTryInput(Machine machine, int tid, EInputType inputType, GameObject inputObject)
    {
        bool success = true;

        switch (inputType)
        {
            case EInputType.Ingredient:
                IngredientData ingredientData = DataTable.Instance.GetIngredientData(tid);
                if(ingredientData.AvailableMachineTID != machine.Data.TID)
                {
                    success = false;
                }
                break;
            case EInputType.Output:
                OutputData outputData = DataTable.Instance.GetOutputData(tid);
                if(outputData.AvailableMachineTIDList.Contains(machine.Data.TID) == false)
                {
                    success = false;
                }
                break;
            default:
                success = false;
                break;
        }

        if (machine.InputTIDList.Count + 1 > machine.Data.MaxInputCount ||
            machine.IsProcessFinished ||
            machine.InputTIDList.Contains(tid))
        {
            success = false;
        }


        if(success == false)
        {
            return false;
        }

        machine.ServerSetInputType(inputType);
        machine.ServerAddInputTID(tid);
        AudioNetworkManager.Instance.RpcPlaySFX(EMachineAudioType.In);
        return true;
    }
}
