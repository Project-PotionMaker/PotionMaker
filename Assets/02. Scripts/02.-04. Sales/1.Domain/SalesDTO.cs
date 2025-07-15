using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SalesDTO
{
    public readonly int TotalSales;
    public readonly int DailySales;

    public readonly Dictionary<EPotionType, int> SalesVolumeDict;

    //public Dictionary<EPotionType, int> SalesVolumeDict => SalesVolumeKeyValueList.ToDictionary(kv => kv.Key, kv => kv.Value);

    public SalesDTO(Sales sales)
    {
        TotalSales = sales.TotalSales;
        DailySales = sales.DailySales;
        SalesVolumeDict = sales.SalesVolumeDict;
        //SalesVolumeKeyValueList = sales.SalesVolumeDict.Select(kv => new SalesVolumeKeyValue { Key = kv.Key, Value = kv.Value }).ToList();
    }

    public SalesDTO(int totalSales, int dailySales, Dictionary<EPotionType, int> salesVolumeDict)
    {
        DailySales = dailySales;
        TotalSales = totalSales;
        SalesVolumeDict = salesVolumeDict;
        //SalesVolumeKeyValueList = salesVolumeDict.Select(kv => new SalesVolumeKeyValue { Key = kv.Key, Value = kv.Value }).ToList();
    }
}
