using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class StructureManager : MonoBehaviourSingleton<StructureManager>
{
    private const string ADDRESSABLE_KEY_PREFIX = "Prefab_";

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

    //public void InitFurniture(GameObject instance, int furnitureTID)
    //{
    //    FurnitureData furnitureData = DataTable.Instance.GetFurnitureData(furnitureTID);
    //    IInteractable<Furniture, FurnitureStat> interactable = null;
    //    IInputContainer<Furniture, FurnitureStat> inputContainer = null;
    //    IOutputContainer<Furniture, FurnitureStat> outputContainer = null;
    //    ICustomerEffectable<Furniture, FurnitureStat> customerEffectable = null;
    //    // 테스트용
    //    if (furnitureData.Name == "계산기")
    //    {
    //        interactable = new CasherInteract();
    //    }
    //    if(furnitureData.Name == "픽업 테이블")
    //    {
    //        inputContainer = new PickUpTableInputContainer();
    //        outputContainer = new PickUpTableOutputContainer();
    //    }
    //    if(furnitureData.Name == "허름한 의자" || furnitureData.Name == "푹신한 의자")
    //    {
    //        customerEffectable = new ChairEffect();
    //    }
    //    instance.GetComponent<Furniture>().InitFurniture(furnitureData, interactable, inputContainer, outputContainer, customerEffectable);
    //}

    //public void InitMachine(GameObject instance, int MachineTID)
    //{
    //    MachineData machineData = DataTable.Instance.GetMachineData(MachineTID);
    //    IInteractable<Machine, MachineStat> interactable = GetInteractableComponent(machineData.InteractType);
    //    IInputContainer<Machine, MachineStat> inputContainer = new MachineInputContainer();
    //    IOutputContainer<Machine, MachineStat> outputContainer = new MachineOutputContainer();
    //}

    //public void InitStorage(GameObject instance, int storageTID, int ingredientTID)
    //{
    //    StorageData storageData = DataTable.Instance.GetStorageData(storageTID);
    //    IOutputContainer<Storage, StorageStat> outputInteractable = new StorageOutputContainer();
    //    instance.GetComponent<Storage>().InitStorage(storageData, ingredientTID, outputInteractable);
    //}

    //private IInteractable<Machine, MachineStat> GetInteractableComponent(EInteractType interactType)
    //{
    //    switch (interactType)
    //    {
    //        case EInteractType.KeepPressing:
    //            // 수정 필요
    //            return new AutoProgressInteract();
    //        case EInteractType.AutoProgress:
    //            return new AutoProgressInteract();
    //        case EInteractType.ClickRepeatly:
    //            return new ClickRepeatlyInteract();
    //        case EInteractType.ClickOnce:
    //            return new ClickOnceInteract();
    //    }

    //    return null;
    //}
}
