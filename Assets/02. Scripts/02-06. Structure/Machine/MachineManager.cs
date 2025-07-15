using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineManager : MonoBehaviourSingleton<MachineManager>
{
    [SerializeField]
    private List<MachineEntry> _machinePrefabEntryList;

    private Dictionary<int, GameObject> _machinePrefabDict;

    protected override void Awake()
    {
        base.Awake();

        if (_machinePrefabEntryList == null || _machinePrefabEntryList.Count == 0)
        {
            _machinePrefabDict = new Dictionary<int, GameObject>();
            return;
        }

        try
        {
            _machinePrefabDict = _machinePrefabEntryList.ToDictionary(entry => entry.TID, entry => entry.Prefab);
        }
        catch (System.ArgumentException ex)
        {
            Debug.LogError($"Duplicate TID found in MachinePrefabEntries: {ex.Message}");
        }
        Debug.Log($"MachineManager: Initialized with {_machinePrefabDict.Count} machines.");
    }

    public GameObject GetMachinePrefab(int tid)
    {
        if(_machinePrefabDict.TryGetValue(tid, out GameObject prefab))
        {
            return prefab;
        }
        return null;
    }
}
