using UnityEngine;

public class DefaultMachineContainer : IMachineItemContainer
{
    public GameObject TakeOutput(Machine machine, MachineStat stat)
    {
        if (stat.IsProcessFinished)
        {
            //여기 Machine에 합쳐버리면 시트 테이블 타입 주는 곳에서 어떻게 판별할까?
            GameObject output = OutputManager.Instance.TryCreateOutput
                (stat.InputTIDList, stat.Data.TID, EInputType.Output, machine.transform.position);
            stat.LeftOutputAmount--;
            if (stat.LeftOutputAmount <= 0)
            {
                stat.ClearMachine();
            }

            machine.SyncMachineStat();
            return output;
        }

        return null;
    }

    public bool TryInput(Machine machine, MachineStat stat, int tid, EInputType inputType)
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
