using Unity.VisualScripting;
using UnityEngine;

public class MachineInputContainer : IInputContainer<Machine, MachineStat>
{
    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if (stat.InputTIDList.Count + 1 > stat.Data.MaxInputCount ||
            stat.IsProcessFinished ||
            stat.InputTIDList.Contains(tid))
        {
            return false;
        }

        stat.InputTIDList.Add(tid);

        machine.SyncMachineStat();
        return true;
    }
}
