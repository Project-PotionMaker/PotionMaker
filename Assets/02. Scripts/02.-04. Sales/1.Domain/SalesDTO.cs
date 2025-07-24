using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SalesDTO
{
    public readonly int TotalSales;
    public readonly int DailySales;

    public readonly Dictionary<EPotionType, int> TotalSalesVolumeDict;
    public readonly Dictionary<EPotionType, int> DailySalesVolumeDict;


    //public Dictionary<EPotionType, int> SalesVolumeDict => SalesVolumeKeyValueList.ToDictionary(kv => kv.Key, kv => kv.Value);

    public SalesDTO(Sales sales)
    {
        TotalSales = sales.TotalSales;
        DailySales = sales.DailySales;
        TotalSalesVolumeDict = sales.TotalSalesVolumeDict;
        DailySalesVolumeDict = sales.DailySalesVolumeDict;

        //SalesVolumeKeyValueList = sales.SalesVolumeDict.Select(kv => new SalesVolumeKeyValue { Key = kv.Key, Value = kv.Value }).ToList();
    }

    public SalesDTO(int totalSales, int dailySales, Dictionary<EPotionType, int> totalSalesVolumeDict, Dictionary<EPotionType, int> dailySalesVolumeDict)
    {
        DailySales = dailySales;
        TotalSales = totalSales;
        TotalSalesVolumeDict = totalSalesVolumeDict;
        DailySalesVolumeDict = dailySalesVolumeDict;

        //SalesVolumeKeyValueList = salesVolumeDict.Select(kv => new SalesVolumeKeyValue { Key = kv.Key, Value = kv.Value }).ToList();
    }
}
