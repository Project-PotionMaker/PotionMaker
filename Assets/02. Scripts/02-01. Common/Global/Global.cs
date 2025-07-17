using UnityEngine;
using System;
using System.Collections;

public class Global : MonoBehaviourSingleton<Global>
{
    public Action OnDataLoaded;
    private bool _isDataLoaded;
    public bool IsDataLoaded => _isDataLoaded;
    private IEnumerator Start()
    {
        yield return DataTable.Instance.Load_Routine();
        OnDataLoaded?.Invoke();
        _isDataLoaded = true;
    }
}
