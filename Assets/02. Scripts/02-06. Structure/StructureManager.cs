using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : NetworkBehaviourSingleton<StructureManager>
{
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
