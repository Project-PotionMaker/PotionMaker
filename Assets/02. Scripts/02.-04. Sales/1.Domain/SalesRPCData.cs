using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SalesRPCData
{
    [Serializable]
    public class SalesVolumeKeyValue
    {
        public EPotionType Key;
        public int Value;
    }

    public int TotalSales;
    public int DailySales;
    public List<SalesVolumeKeyValue> SalesVolumeKeyValueList;

    public SalesRPCData(SalesDTO salesDTO)
    {
        TotalSales = salesDTO.TotalSales;
        DailySales = salesDTO.DailySales;

        SalesVolumeKeyValueList = new List<SalesVolumeKeyValue>();
        foreach (var keyValuePair in salesDTO.SalesVolumeDict)
        {
            SalesVolumeKeyValueList.Add(new SalesVolumeKeyValue
            {
                Key = keyValuePair.Key,
                Value = keyValuePair.Value
            });
        }
    }
}
