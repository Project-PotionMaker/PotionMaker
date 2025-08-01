using System.Linq;
using UnityEngine;

public class MachineOutputContainer : IOutputContainer<Machine>
{
    private GameObject _output;

    public GameObject ServerTakeItem(Machine machine)
    {
        if (machine.IsProcessFinished)
        {
            if (machine.Data.Name == "병입기")
            {
                _output = CraftItemManager.Instance.TryCreatePotionItem(machine.InputTIDList.ToArray(), machine.DataTID, machine.transform.position);
            }
            else
            {
                _output = CraftItemManager.Instance.TryCreateOutputItem(machine.InputTIDList.ToArray(), machine.DataTID, machine.InputType, machine.transform.position);
            }

            machine.ServerDecreaseOutputAmount(1);
            if (machine.LeftOutputAmount <= 0)
            {
                machine.ResetMachineServer();
            }

            return _output;
        }
        return null;
    }

    public bool ServerCanTake(Machine machine)
    {
        return machine.IsProcessFinished;
    }
}
