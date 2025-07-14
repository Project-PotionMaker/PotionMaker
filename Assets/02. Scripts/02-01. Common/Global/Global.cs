using UnityEngine;
using System;
using System.Collections;

public class Global : MonoBehaviourSingleton<Global>
{
    public Action OnDataLoaded;

    private IEnumerator Start()
    {
        yield return DataTable.Instance.Load_Routine();
        OnDataLoaded?.Invoke();

        Debug.Log(DataTable.Instance.GetMachineData(10000).MachineCode);
    }
}
