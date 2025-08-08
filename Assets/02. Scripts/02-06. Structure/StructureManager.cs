using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : NetworkBehaviourSingleton<StructureManager>
{
    private Dictionary<ESpecialStructureType, int> _specialStructureTIDDict = new();
    public Dictionary<ESpecialStructureType, int> SpecialStructureTIDDict => _specialStructureTIDDict;

    public override void OnStartClient()
    {
        ReadOnlyList<StructureData> structureDataList = DataTable.Instance.GetStructureDataList();

        foreach(StructureData data in structureDataList)
        {
            switch (data.SpecialStructureType)
            {
                case ESpecialStructureType.PickUpTable:
                    _specialStructureTIDDict.TryAdd(ESpecialStructureType.PickUpTable, data.TID);
                    break;
                case ESpecialStructureType.TrashCan:
                    _specialStructureTIDDict.TryAdd(ESpecialStructureType.TrashCan, data.TID);
                    break;
                case ESpecialStructureType.Casher:
                    _specialStructureTIDDict.TryAdd(ESpecialStructureType.Casher, data.TID);
                    break;
                case ESpecialStructureType.LuxuryChair:
                    _specialStructureTIDDict.TryAdd(ESpecialStructureType.LuxuryChair, data.TID);
                    break;
                case ESpecialStructureType.OldChair:
                    _specialStructureTIDDict.TryAdd(ESpecialStructureType.OldChair, data.TID);
                    break;
            }
        }
    }

    [Server]
    public GameObject ServerCreateStructure(int structureTID, int ingredientTID = 10000)
    {
        StructureData data = DataTable.Instance.GetStructureData(structureTID);
        GameObject instance = StructureFactory.Instance.CreateObject(data.StructureType, Vector3.zero, Quaternion.identity);

        switch (data.StructureType)
        {
            case EStructureType.Furniture:
                instance.GetComponent<Furniture>().ServerInitFurniture(data.TypeTID);
                break;
            case EStructureType.Machine:
                instance.GetComponent<Machine>().ServerInitMachine(data.TypeTID);
                break;
            case EStructureType.Storage:
                instance.GetComponent<Storage>().ServerInitStorage(data.TypeTID, ingredientTID);
                break;
            case EStructureType.None:
                Destroy(instance);
                return null;
        }
        return instance;
    }
}
