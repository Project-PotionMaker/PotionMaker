using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class StructureManager : MonoBehaviourSingleton<StructureManager>
{
    private const string ADDRESSABLE_KEY_PREFIX = "Prefab_Structure_";

    private async void Start()
    {
        
        while(DataTable.Instance.GetStructureDataList() == null || PhotonNetwork.InRoom == false)
        {
            await Task.Delay(10);
        }
        GameObject structure = CreateStructure(10000);
        structure.transform.position = Vector3.one;
    }



    public GameObject CreateStructure(int structureTID)
    {
        // 포톤네트워크로 생성 + 오브젝트풀 고려
        GameObject instance = StructureFactory.Instance.Create($"{ADDRESSABLE_KEY_PREFIX}{structureTID}", Vector3.zero, Quaternion.identity);
        StructureData data = DataTable.Instance.GetStructureData(structureTID);
        switch (data.StructureType)
        {
            case EStructureType.Machine:
                SetMachine(instance, data.TypeTID, structureTID);
                break;
            case EStructureType.Furniture:
                FurnitureData furnitureData = DataTable.Instance.GetFurnitureData(data.TypeTID);
                instance.GetComponent<Furniture>().Init(furnitureData);
                break;
            case EStructureType.None:
                Destroy(instance);
                return null;
        }

        return instance;
    }

    public bool TryDeleteStructure(GameObject structureObject)
    {
        if(structureObject == null)
        {
            return false;
        }

        // 추가 처리사항 있으면 여기서 ㄱㄱ
        Destroy(structureObject);
        return true;
    }

    public async Task<GameObject> GetStructurePrefab(int structureTID)
    {
        GameObject prefab = await AssetManager.Instance.LoadAsset<GameObject>($"{ADDRESSABLE_KEY_PREFIX}{structureTID}");
        return prefab;
    }

    [PunRPC]
    public void SetMachine(GameObject instance, int detailDataTID, int structureTID)
    {
        MachineData machineData = DataTable.Instance.GetMachineData(detailDataTID);
        IMachineInteractable machineInteractable = GetMachineInteractableComponent(machineData.InteractType);
        IMachineItemContainer machineItemContainer = GetMachineItemContainerComponent(structureTID);
        instance.GetComponent<Machine>().InitMachine(machineData, machineInteractable, machineItemContainer);
    }

    private IMachineInteractable GetMachineInteractableComponent(EInteractType interactType)
    {
        switch (interactType)
        {
            case EInteractType.KeepPressing:
                break;
            case EInteractType.AutoProgress:
                return new AutoProgressInteract();
            case EInteractType.ClickRepeatly:
                break;
            case EInteractType.ClickOnce:
                break;
        }

        return null;
    }

    private IMachineItemContainer GetMachineItemContainerComponent(int tid)
    {
        return new DefaultMachineContainer();
    }
    ////테스트용 코드입니다.
    //[SerializeField]
    //private List<MachineEntry> _machinePrefabEntryList;

    private Dictionary<int, GameObject> _machinePrefabDict;

    //protected override void Awake()
    //{
    //    base.Awake();

    //    if (_machinePrefabEntryList == null || _machinePrefabEntryList.Count == 0)
    //    {
    //        _machinePrefabDict = new Dictionary<int, GameObject>();
    //        return;
    //    }

    //    try
    //    {
    //        _machinePrefabDict = _machinePrefabEntryList.ToDictionary(entry => entry.TID, entry => entry.Prefab);
    //    }
    //    catch (System.ArgumentException ex)
    //    {
    //        Debug.LogError($"Duplicate TID found in MachinePrefabEntries: {ex.Message}");
    //    }
    //    Debug.Log($"MachineManager: Initialized with {_machinePrefabDict.Count} machines.");
    //}

    public GameObject GetMachinePrefab(int tid)
    {
        //if (_machinePrefabDict.TryGetValue(tid, out GameObject prefab))
        //{
        //    return prefab;
        //}
        return null;
    }

}
