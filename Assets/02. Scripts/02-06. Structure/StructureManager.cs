using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : NetworkBehaviourSingleton<StructureManager>
{
    [Server]
    public GameObject CreateStructure(int structureTID, int ingredientTID = 10000)
    {
        StructureData data = DataTable.Instance.GetStructureData(structureTID);
        GameObject instance = StructureFactory.Instance.Create(data.StructureType, Vector3.zero, Quaternion.identity);

        switch (data.StructureType)
        {
            case EStructureType.Furniture:
                instance.GetComponent<Furniture>().RpcInitFurnitureOnClients(data.TypeTID);
                break;
            case EStructureType.Machine:
                instance.GetComponent<Machine>().ServerInitMachine(data.TypeTID);
                break;
            case EStructureType.Storage:
                instance.GetComponent<Storage>().RpcInitStorageOnClients(data.TypeTID, ingredientTID);
                break;
            case EStructureType.None:
                Destroy(instance);
                return null;
        }
        return instance;
    }
}
