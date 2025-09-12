using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SalesDTO
{
    public readonly int TotalSales;
    public readonly int DailySales;

    public readonly Dictionary<int, int> TotalSalesVolumeDict;
    public readonly Dictionary<int, int> DailySalesVolumeDict;


    //public Dictionary<EPotionType, int> SalesVolumeDict => SalesVolumeKeyValueList.ToDictionary(kv => kv.Key, kv => kv.Value);

    public SalesDTO(Sales sales)
    {
        TotalSales = sales.TotalSales;
        DailySales = sales.DailySales;
        TotalSalesVolumeDict = sales.TotalSalesVolumeDict;
        DailySalesVolumeDict = sales.DailySalesVolumeDict;

        //SalesVolumeKeyValueList = sales.SalesVolumeDict.Select(kv => new SalesVolumeKeyValue { Key = kv.Key, Value = kv.Value }).ToList();
    }

    public SalesDTO(int totalSales, int dailySales, Dictionary<int, int> totalSalesVolumeDict, Dictionary<int, int> dailySalesVolumeDict)
    {
        DailySales = dailySales;
        TotalSales = totalSales;
        TotalSalesVolumeDict = totalSalesVolumeDict;
        DailySalesVolumeDict = dailySalesVolumeDict;

        //SalesVolumeKeyValueList = salesVolumeDict.Select(kv => new SalesVolumeKeyValue { Key = kv.Key, Value = kv.Value }).ToList();
    }

    public int GetTotalSalesVolume()
    {
        int sum = 0;
        foreach (int n in TotalSalesVolumeDict.Values)
        {
            sum += n;
        }
        return sum;
    }

    public int GetDailySalesVolume()
    {
        int sum = 0;
        foreach (int n in DailySalesVolumeDict.Values)
        {
            sum += n;
        }
        return sum;
    }
}
