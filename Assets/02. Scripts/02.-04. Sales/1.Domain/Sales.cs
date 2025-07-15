using System.Collections.Generic;

public class Sales
{
    private int _totalSales;
    public int TotalSales => _totalSales;
    private int _dailySales;
    public int DailySales => _dailySales;

    private Dictionary<EPotionType, int> _salesVolumeDict;
    public Dictionary<EPotionType, int> SalesVolumeDict => _salesVolumeDict;
    public Sales(int totalSales, int dailySales = 0, Dictionary<EPotionType, int> salesVolumeDict = null)
    {
        _totalSales = totalSales;
        _dailySales = dailySales;

        if (ReferenceEquals(salesVolumeDict, null))
        {
            _salesVolumeDict = new Dictionary<EPotionType, int>();
        }
        else
        {
            _salesVolumeDict = salesVolumeDict;
        }
    }
    public int GetTotalSalesVolumeDictionary()
    {
        int sum = 0;
        foreach (int n in _salesVolumeDict.Keys)
        {
            sum += n;
        }
        return sum;
    }

    public void Sell(EPotionType potionType, int price)
    {
        if (!_salesVolumeDict.ContainsKey(potionType))
        {
            _salesVolumeDict.Add(potionType, 0);
        }
        ++_salesVolumeDict[potionType];
        _dailySales += price;
        _totalSales += price;
    }

    public void SetSales(int totalSales, int dailySales, Dictionary<EPotionType, int> salesVolumeDict)
    {
        _totalSales = totalSales;
        _dailySales = dailySales;
        _salesVolumeDict = salesVolumeDict;
    }

    public void ResetDailySales()
    {
        _dailySales = 0;
    }

    public SalesDTO ToDTO()
    {
        return new SalesDTO(this);
    }

}
