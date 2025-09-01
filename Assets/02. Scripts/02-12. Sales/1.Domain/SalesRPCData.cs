using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SalesRPCData
{
    [Serializable]
    public class SalesVolumeKeyValue
    {
        public int Key;
        public int Value;
    }

    public int TotalSales;
    public int DailySales;
    public List<SalesVolumeKeyValue> TotalSalesVolumeKeyValueList;
    public List<SalesVolumeKeyValue> DailySalesVolumeKeyValueList;


    public SalesRPCData(SalesDTO salesDTO)
    {
        TotalSales = salesDTO.TotalSales;
        DailySales = salesDTO.DailySales;

        TotalSalesVolumeKeyValueList = new List<SalesVolumeKeyValue>(salesDTO.TotalSalesVolumeDict.Count);
        foreach (var keyValuePair in salesDTO.TotalSalesVolumeDict)
        {
            TotalSalesVolumeKeyValueList.Add(new SalesVolumeKeyValue
            {
                Key = keyValuePair.Key,
                Value = keyValuePair.Value
            });
        }
        DailySalesVolumeKeyValueList = new List<SalesVolumeKeyValue>(salesDTO.DailySalesVolumeDict.Count);
        foreach (var keyValuePair in salesDTO.DailySalesVolumeDict)
        {
            DailySalesVolumeKeyValueList.Add(new SalesVolumeKeyValue
            {
                Key = keyValuePair.Key,
                Value = keyValuePair.Value
            });
        }
    }
}
