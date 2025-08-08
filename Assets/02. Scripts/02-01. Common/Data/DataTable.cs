// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class DataTable
{
    #region Ingredient
    private ReadOnlyList<IngredientData> IngredientList = null;
    private ReadOnlyDictionary<int, IngredientData> IngredientTable = null;

    public ReadOnlyList<IngredientData> GetIngredientDataList()
    {
        return IngredientList;
    }

    public IngredientData GetIngredientData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (IngredientTable.TryGetValue(key, out IngredientData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of IngredientData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Potion
    private ReadOnlyList<PotionData> PotionList = null;
    private ReadOnlyDictionary<int, PotionData> PotionTable = null;

    public ReadOnlyList<PotionData> GetPotionDataList()
    {
        return PotionList;
    }

    public PotionData GetPotionData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (PotionTable.TryGetValue(key, out PotionData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of PotionData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Output
    private ReadOnlyList<OutputData> OutputList = null;
    private ReadOnlyDictionary<int, OutputData> OutputTable = null;

    public ReadOnlyList<OutputData> GetOutputDataList()
    {
        return OutputList;
    }

    public OutputData GetOutputData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (OutputTable.TryGetValue(key, out OutputData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of OutputData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Product
    private ReadOnlyList<ProductData> ProductList = null;
    private ReadOnlyDictionary<int, ProductData> ProductTable = null;

    public ReadOnlyList<ProductData> GetProductDataList()
    {
        return ProductList;
    }

    public ProductData GetProductData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (ProductTable.TryGetValue(key, out ProductData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of ProductData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Structure
    private ReadOnlyList<StructureData> StructureList = null;
    private ReadOnlyDictionary<int, StructureData> StructureTable = null;

    public ReadOnlyList<StructureData> GetStructureDataList()
    {
        return StructureList;
    }

    public StructureData GetStructureData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (StructureTable.TryGetValue(key, out StructureData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of StructureData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Machine
    private ReadOnlyList<MachineData> MachineList = null;
    private ReadOnlyDictionary<int, MachineData> MachineTable = null;

    public ReadOnlyList<MachineData> GetMachineDataList()
    {
        return MachineList;
    }

    public MachineData GetMachineData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (MachineTable.TryGetValue(key, out MachineData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of MachineData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Storage
    private ReadOnlyList<StorageData> StorageList = null;
    private ReadOnlyDictionary<int, StorageData> StorageTable = null;

    public ReadOnlyList<StorageData> GetStorageDataList()
    {
        return StorageList;
    }

    public StorageData GetStorageData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (StorageTable.TryGetValue(key, out StorageData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of StorageData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Furniture
    private ReadOnlyList<FurnitureData> FurnitureList = null;
    private ReadOnlyDictionary<int, FurnitureData> FurnitureTable = null;

    public ReadOnlyList<FurnitureData> GetFurnitureDataList()
    {
        return FurnitureList;
    }

    public FurnitureData GetFurnitureData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (FurnitureTable.TryGetValue(key, out FurnitureData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of FurnitureData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Unlock
    private ReadOnlyList<UnlockData> UnlockList = null;
    private ReadOnlyDictionary<int, UnlockData> UnlockTable = null;

    public ReadOnlyList<UnlockData> GetUnlockDataList()
    {
        return UnlockList;
    }

    public UnlockData GetUnlockData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (UnlockTable.TryGetValue(key, out UnlockData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of UnlockData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Layout
    private ReadOnlyList<LayoutData> LayoutList = null;
    private ReadOnlyDictionary<int, LayoutData> LayoutTable = null;

    public ReadOnlyList<LayoutData> GetLayoutDataList()
    {
        return LayoutList;
    }

    public LayoutData GetLayoutData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (LayoutTable.TryGetValue(key, out LayoutData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of LayoutData: <{key}>");
            return null;
        }
    }
    #endregion
    #region Tip
    private ReadOnlyList<TipData> TipList = null;
    private ReadOnlyDictionary<int, TipData> TipTable = null;

    public ReadOnlyList<TipData> GetTipDataList()
    {
        return TipList;
    }

    public TipData GetTipData(int key)
    {
        if (key == 0)
        {
            return null;
        }

        if (TipTable.TryGetValue(key, out TipData retVal) == true)
        {
            return retVal;
        }
        else
        {
            Debug.LogError($"Can not find UniqueID of TipData: <{key}>");
            return null;
        }
    }
    #endregion

    public IEnumerator LoadRoutine()
    {
        int allCount = 0;
        int loadedCount = 0;

        allCount++;
        GetBytes_FromResources("Ingredient", (bytes) =>
        {
            LoadIngredientData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Potion", (bytes) =>
        {
            LoadPotionData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Output", (bytes) =>
        {
            LoadOutputData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Product", (bytes) =>
        {
            LoadProductData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Structure", (bytes) =>
        {
            LoadStructureData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Machine", (bytes) =>
        {
            LoadMachineData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Storage", (bytes) =>
        {
            LoadStorageData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Furniture", (bytes) =>
        {
            LoadFurnitureData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Unlock", (bytes) =>
        {
            LoadUnlockData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Layout", (bytes) =>
        {
            LoadLayoutData(bytes);
            loadedCount++;
        });
        allCount++;
        GetBytes_FromResources("Tip", (bytes) =>
        {
            LoadTipData(bytes);
            loadedCount++;
        });

        yield return new WaitUntil(() => allCount == loadedCount);
    }

    public void LoadForEditor()
    {
        byte[] ingredientBytes = GetBytes_ForEditor("IngredientData");
        LoadIngredientData(ingredientBytes);
        byte[] potionBytes = GetBytes_ForEditor("PotionData");
        LoadPotionData(potionBytes);
        byte[] outputBytes = GetBytes_ForEditor("OutputData");
        LoadOutputData(outputBytes);
        byte[] productBytes = GetBytes_ForEditor("ProductData");
        LoadProductData(productBytes);
        byte[] structureBytes = GetBytes_ForEditor("StructureData");
        LoadStructureData(structureBytes);
        byte[] machineBytes = GetBytes_ForEditor("MachineData");
        LoadMachineData(machineBytes);
        byte[] storageBytes = GetBytes_ForEditor("StorageData");
        LoadStorageData(storageBytes);
        byte[] furnitureBytes = GetBytes_ForEditor("FurnitureData");
        LoadFurnitureData(furnitureBytes);
        byte[] unlockBytes = GetBytes_ForEditor("UnlockData");
        LoadUnlockData(unlockBytes);
        byte[] layoutBytes = GetBytes_ForEditor("LayoutData");
        LoadLayoutData(layoutBytes);
        byte[] tipBytes = GetBytes_ForEditor("TipData");
        LoadTipData(tipBytes);
    }

    private void LoadIngredientData(byte[] bytes)
    {
        List<IngredientData> ingredientList = new List<IngredientData>();
        Dictionary<int, IngredientData> ingredientTable = new Dictionary<int, IngredientData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            IngredientData data = new IngredientData(Reader);
            if (ingredientTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Ingredient");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Ingredient");
                continue;
            }

            ingredientList.Add(data);
            ingredientTable.Add(data.TID, data);
        }

        Reader.Close();

        IngredientList = new ReadOnlyList<IngredientData>(ingredientList);
        IngredientTable = new ReadOnlyDictionary<int, IngredientData>(ingredientTable);
    }

    private void LoadPotionData(byte[] bytes)
    {
        List<PotionData> potionList = new List<PotionData>();
        Dictionary<int, PotionData> potionTable = new Dictionary<int, PotionData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            PotionData data = new PotionData(Reader);
            if (potionTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Potion");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Potion");
                continue;
            }

            potionList.Add(data);
            potionTable.Add(data.TID, data);
        }

        Reader.Close();

        PotionList = new ReadOnlyList<PotionData>(potionList);
        PotionTable = new ReadOnlyDictionary<int, PotionData>(potionTable);
    }

    private void LoadOutputData(byte[] bytes)
    {
        List<OutputData> outputList = new List<OutputData>();
        Dictionary<int, OutputData> outputTable = new Dictionary<int, OutputData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            OutputData data = new OutputData(Reader);
            if (outputTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Output");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Output");
                continue;
            }

            outputList.Add(data);
            outputTable.Add(data.TID, data);
        }

        Reader.Close();

        OutputList = new ReadOnlyList<OutputData>(outputList);
        OutputTable = new ReadOnlyDictionary<int, OutputData>(outputTable);
    }

    private void LoadProductData(byte[] bytes)
    {
        List<ProductData> productList = new List<ProductData>();
        Dictionary<int, ProductData> productTable = new Dictionary<int, ProductData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            ProductData data = new ProductData(Reader);
            if (productTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Product");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Product");
                continue;
            }

            productList.Add(data);
            productTable.Add(data.TID, data);
        }

        Reader.Close();

        ProductList = new ReadOnlyList<ProductData>(productList);
        ProductTable = new ReadOnlyDictionary<int, ProductData>(productTable);
    }

    private void LoadStructureData(byte[] bytes)
    {
        List<StructureData> structureList = new List<StructureData>();
        Dictionary<int, StructureData> structureTable = new Dictionary<int, StructureData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            StructureData data = new StructureData(Reader);
            if (structureTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Structure");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Structure");
                continue;
            }

            structureList.Add(data);
            structureTable.Add(data.TID, data);
        }

        Reader.Close();

        StructureList = new ReadOnlyList<StructureData>(structureList);
        StructureTable = new ReadOnlyDictionary<int, StructureData>(structureTable);
    }

    private void LoadMachineData(byte[] bytes)
    {
        List<MachineData> machineList = new List<MachineData>();
        Dictionary<int, MachineData> machineTable = new Dictionary<int, MachineData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            MachineData data = new MachineData(Reader);
            if (machineTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Machine");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Machine");
                continue;
            }

            machineList.Add(data);
            machineTable.Add(data.TID, data);
        }

        Reader.Close();

        MachineList = new ReadOnlyList<MachineData>(machineList);
        MachineTable = new ReadOnlyDictionary<int, MachineData>(machineTable);
    }

    private void LoadStorageData(byte[] bytes)
    {
        List<StorageData> storageList = new List<StorageData>();
        Dictionary<int, StorageData> storageTable = new Dictionary<int, StorageData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            StorageData data = new StorageData(Reader);
            if (storageTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Storage");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Storage");
                continue;
            }

            storageList.Add(data);
            storageTable.Add(data.TID, data);
        }

        Reader.Close();

        StorageList = new ReadOnlyList<StorageData>(storageList);
        StorageTable = new ReadOnlyDictionary<int, StorageData>(storageTable);
    }

    private void LoadFurnitureData(byte[] bytes)
    {
        List<FurnitureData> furnitureList = new List<FurnitureData>();
        Dictionary<int, FurnitureData> furnitureTable = new Dictionary<int, FurnitureData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            FurnitureData data = new FurnitureData(Reader);
            if (furnitureTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Furniture");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Furniture");
                continue;
            }

            furnitureList.Add(data);
            furnitureTable.Add(data.TID, data);
        }

        Reader.Close();

        FurnitureList = new ReadOnlyList<FurnitureData>(furnitureList);
        FurnitureTable = new ReadOnlyDictionary<int, FurnitureData>(furnitureTable);
    }

    private void LoadUnlockData(byte[] bytes)
    {
        List<UnlockData> unlockList = new List<UnlockData>();
        Dictionary<int, UnlockData> unlockTable = new Dictionary<int, UnlockData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            UnlockData data = new UnlockData(Reader);
            if (unlockTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Unlock");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Unlock");
                continue;
            }

            unlockList.Add(data);
            unlockTable.Add(data.TID, data);
        }

        Reader.Close();

        UnlockList = new ReadOnlyList<UnlockData>(unlockList);
        UnlockTable = new ReadOnlyDictionary<int, UnlockData>(unlockTable);
    }

    private void LoadLayoutData(byte[] bytes)
    {
        List<LayoutData> layoutList = new List<LayoutData>();
        Dictionary<int, LayoutData> layoutTable = new Dictionary<int, LayoutData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            LayoutData data = new LayoutData(Reader);
            if (layoutTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Layout");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Layout");
                continue;
            }

            layoutList.Add(data);
            layoutTable.Add(data.TID, data);
        }

        Reader.Close();

        LayoutList = new ReadOnlyList<LayoutData>(layoutList);
        LayoutTable = new ReadOnlyDictionary<int, LayoutData>(layoutTable);
    }

    private void LoadTipData(byte[] bytes)
    {
        List<TipData> tipList = new List<TipData>();
        Dictionary<int, TipData> tipTable = new Dictionary<int, TipData>();

        Reader = new BinaryReader(new MemoryStream(bytes));

        while (Reader.BaseStream.Position < bytes.Length)
        {
            TipData data = new TipData(Reader);
            if (tipTable.ContainsKey(data.TID) == true)
            {
                Debug.LogError("The duplicate TID: " + data.TID + " in Tip");
                continue;
            }
            else if (data.TID == 0)
            {
                Debug.LogError("TID is 0 in Tip");
                continue;
            }

            tipList.Add(data);
            tipTable.Add(data.TID, data);
        }

        Reader.Close();

        TipList = new ReadOnlyList<TipData>(tipList);
        TipTable = new ReadOnlyDictionary<int, TipData>(tipTable);
    }

}
