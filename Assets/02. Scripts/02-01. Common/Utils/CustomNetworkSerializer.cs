using Mirror;
using System.Collections.Generic;
using UnityEngine;

public static class CustomNetworkSerializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // GridSaveData
        Writer<GridSaveData>.write = WriteGridSaveData;
        Reader<GridSaveData>.read = ReadGridSaveData;

        // Currency
        Writer<Currency>.write = WriteCurrency;
        Reader<Currency>.read = ReadCurrency;

        // Reputation
        Writer<Reputation>.write = WriteReputation;
        Reader<Reputation>.read = ReadReputation;

        // Sales
        Writer<Sales>.write = WriteSales;
        Reader<Sales>.read = ReadSales;

        // Rent 추가
        Writer<Rent>.write = WriteRent;
        Reader<Rent>.read = ReadRent;

        // ShopInfo
        Writer<ShopInfo>.write = WriteShopInfo;
        Reader<ShopInfo>.read = ReadShopInfo;
    }

    public static void WriteGridSaveData(this NetworkWriter writer, GridSaveData data)
    {
        writer.WriteVector3Int(data.GridPosition);
        writer.WriteInt(data.StructureTID);
        writer.WriteInt(data.IngredientTID);
    }

    public static GridSaveData ReadGridSaveData(this NetworkReader reader)
    {
        return new GridSaveData(
            reader.ReadVector3Int(),
            reader.ReadInt(),
            reader.ReadInt()
        );
    }

    public static void WriteCurrency(this NetworkWriter writer, Currency currency)
    {
        writer.WriteInt(currency.Value);
    }

    public static Currency ReadCurrency(this NetworkReader reader)
    {
        return new Currency(reader.ReadInt());
    }

    public static void WriteReputation(this NetworkWriter writer, Reputation reputation)
    {
        writer.WriteFloat(reputation.Value);
        writer.WriteFloat(reputation.ValueYesterday);
        writer.WriteFloat(reputation.Difference);
        writer.WriteInt((int)reputation.ReputationGrade);
    }

    public static Reputation ReadReputation(this NetworkReader reader)
    {
        float value = reader.ReadFloat();
        float valueYesterday = reader.ReadFloat();
        float difference = reader.ReadFloat();
        EReputationGrade grade = (EReputationGrade)reader.ReadInt();

        return new Reputation(value, valueYesterday, difference, grade);
    }

    private static void WriteDictionaryIntInt(this NetworkWriter writer, Dictionary<int, int> dict)
    {
        writer.WriteInt(dict?.Count ?? 0);
        if (dict == null) return;

        foreach (var kvp in dict)
        {
            writer.WriteInt(kvp.Key);
            writer.WriteInt(kvp.Value);
        }
    }

    private static Dictionary<int, int> ReadDictionaryIntInt(this NetworkReader reader)
    {
        int count = reader.ReadInt();
        var dict = new Dictionary<int, int>(count);
        for (int i = 0; i < count; i++)
        {
            dict.Add(reader.ReadInt(), reader.ReadInt());
        }
        return dict;
    }

    public static void WriteSales(this NetworkWriter writer, Sales sales)
    {
        writer.WriteInt(sales.TotalSales);
        writer.WriteInt(sales.DailySales);
        writer.WriteDictionaryIntInt(sales.TotalSalesVolumeDict);
        writer.WriteDictionaryIntInt(sales.DailySalesVolumeDict);
    }

    public static Sales ReadSales(this NetworkReader reader)
    {
        int totalSales = reader.ReadInt();
        int dailySales = reader.ReadInt();
        var totalSalesVolumeDict = ReadDictionaryIntInt(reader);
        var dailySalesVolumeDict = ReadDictionaryIntInt(reader);

        return new Sales(totalSales, dailySales, totalSalesVolumeDict, dailySalesVolumeDict);
    }

    public static void WriteRent(this NetworkWriter writer, Rent rent)
    {
        writer.WriteInt(rent.RentDayCounter);
        writer.WriteInt(rent.CurrentRentCost);
        writer.WriteInt(rent.RentIncrement);
    }

    public static Rent ReadRent(this NetworkReader reader)
    {
        int rentDayCounter = reader.ReadInt();
        int currentRentCost = reader.ReadInt();
        int rentIncrement = reader.ReadInt();

        return new Rent(rentDayCounter, currentRentCost, rentIncrement);
    }

    public static void WriteShopInfo(this NetworkWriter writer, ShopInfo info)
    {
        writer.WriteString(info.ShopName);
        writer.WriteInt(info.SlotIndex);
        writer.WriteInt(info.Day);
        writer.WriteInt(info.PotionHouseTier);
        writer.Write(info.Currency);
        writer.Write(info.Reputation);
        writer.Write(info.Sales);
        writer.Write(info.Rent);
        writer.WriteList(info.GridSaveDataList);
    }

    public static ShopInfo ReadShopInfo(this NetworkReader reader)
    {
        string shopName = reader.ReadString();
        int slotIndex = reader.ReadInt();
        int day = reader.ReadInt();
        int potionHouseTier = reader.ReadInt();
        Currency currency = reader.Read<Currency>();
        Reputation reputation = reader.Read<Reputation>();
        Sales sales = reader.Read<Sales>();
        Rent rent = reader.Read<Rent>();
        List<GridSaveData> gridSaveDataList = reader.ReadList<GridSaveData>();

        return new ShopInfo(shopName, slotIndex, day, potionHouseTier, currency, reputation, sales, rent, gridSaveDataList);
    }
}