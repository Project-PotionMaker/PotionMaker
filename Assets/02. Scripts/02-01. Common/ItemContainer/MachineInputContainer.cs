using Unity.VisualScripting;
using UnityEngine;

public class MachineInputContainer : IInputContainer<Machine>
{
    public bool TryInput(Machine machine, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if (machine.InputTIDList.Count + 1 > machine.Data.MaxInputCount ||
            machine.IsProcessFinished ||
            machine.InputTIDList.Contains(tid))
        {
            return false;
        }

        machine.InputType = inputType;
        machine.InputTIDList.Add(tid);

        return true;
    }
}
