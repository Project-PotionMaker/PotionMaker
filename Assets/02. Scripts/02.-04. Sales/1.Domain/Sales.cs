using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class Sales
{
    private int _totalSales;
    public int TotalSales => _totalSales;
    private int _dailySales;
    public int DailySales => _dailySales;

    private Dictionary<EPotionType, int> _totalSalesVolumeDict;
    public Dictionary<EPotionType, int> TotalSalesVolumeDict => _totalSalesVolumeDict;

    private Dictionary<EPotionType, int> _dailySalesVolumeDict;
    public Dictionary<EPotionType, int> DailySalesVolumeDict => _dailySalesVolumeDict;
    public Sales(int totalSales, int dailySales = 0, Dictionary<EPotionType, int> totalSalesVolumeDict = null, Dictionary<EPotionType, int> dailySalesVolumeDict = null)
    {
        if (totalSales < 0)
        {
            throw new ArgumentOutOfRangeException
            (
            nameof(totalSales),
            totalSales,
                $"{nameof(totalSales)} must be zero or greater");
        }
        if (dailySales < 0)
        {
            throw new ArgumentOutOfRangeException
            (
            nameof(dailySales),
            dailySales,
                $"{nameof(dailySales)} must be zero or greater");
        }
        _totalSales = totalSales;
        _dailySales = dailySales;

        if (ReferenceEquals(totalSalesVolumeDict, null))
        {
            _totalSalesVolumeDict = new Dictionary<EPotionType, int>();
        }
        else
        {
            _totalSalesVolumeDict = totalSalesVolumeDict;
        }

        if (ReferenceEquals(dailySalesVolumeDict, null))
        {
            _dailySalesVolumeDict = new Dictionary<EPotionType, int>();
        }
        else
        {
            _dailySalesVolumeDict = dailySalesVolumeDict;
        }
    }
    
    public int GetTotalSalesVolume()
    {
        int sum = 0;
        foreach (int n in _totalSalesVolumeDict.Values)
        {
            sum += n;
        }
        return sum;
    }

    public int GetDailySalesVolume()
    {
        int sum = 0;
        foreach (int n in _dailySalesVolumeDict.Values)
        {
            sum += n;
        }
        return sum;
    }

    public void Sell(EPotionType potionType, int price)
    {
        if (!_totalSalesVolumeDict.ContainsKey(potionType))
        {
            _totalSalesVolumeDict.Add(potionType, 0);
        }
        ++_totalSalesVolumeDict[potionType];

        if (!_dailySalesVolumeDict.ContainsKey(potionType))
        {
            _dailySalesVolumeDict.Add(potionType, 0);
        }
        ++_dailySalesVolumeDict[potionType];

        _totalSales += price;
        _dailySales += price;
    }

    public void SetSales(int totalSales, int dailySales, Dictionary<EPotionType, int> totalSalesVolumeDict, Dictionary<EPotionType, int> dailySalesVolumeDict)
    {
        if(totalSales < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalSales),
                totalSales,
                $"{nameof(totalSales)} must be zero or greater");
        }
        if (dailySales < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailySales),
                dailySales,
                $"{nameof(dailySales)} must be zero or greater");
        }
        if (ReferenceEquals(totalSalesVolumeDict, null))
        {
            throw new ArgumentNullException(
                nameof(totalSalesVolumeDict),
                $"{nameof(totalSalesVolumeDict)} must not be null");
        }
        if (ReferenceEquals(dailySalesVolumeDict, null))
        {
            throw new ArgumentNullException(
                nameof(dailySalesVolumeDict),
                $"{nameof(dailySalesVolumeDict)} must not be null");
        }
        _totalSales = totalSales;
        _dailySales = dailySales;
        _totalSalesVolumeDict = totalSalesVolumeDict;
        _dailySalesVolumeDict = dailySalesVolumeDict;
    }

    public void OnDayChanged()
    {
        _dailySales = 0;
        _dailySalesVolumeDict.Clear();
    }

    public SalesDTO ToDTO()
    {
        return new SalesDTO(this);
    }

}
