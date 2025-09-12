using System;
using System.Collections.Generic;

[Serializable]
public class Sales
{
    private int _totalSales;
    public int TotalSales => _totalSales;
    private int _dailySales;
    public int DailySales => _dailySales;

    private Dictionary<int, int> _totalSalesVolumeDict;
    public Dictionary<int, int> TotalSalesVolumeDict => _totalSalesVolumeDict;

    private Dictionary<int, int> _dailySalesVolumeDict;
    public Dictionary<int, int> DailySalesVolumeDict => _dailySalesVolumeDict;
    public Sales(int totalSales, int dailySales = 0, Dictionary<int, int> totalSalesVolumeDict = null, Dictionary<int, int> dailySalesVolumeDict = null)
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
            _totalSalesVolumeDict = new Dictionary<int, int>();
        }
        else
        {
            _totalSalesVolumeDict = totalSalesVolumeDict;
        }

        if (ReferenceEquals(dailySalesVolumeDict, null))
        {
            _dailySalesVolumeDict = new Dictionary<int, int>();
        }
        else
        {
            _dailySalesVolumeDict = dailySalesVolumeDict;
        }
    }
    
    public Sales(SalesDTO salesDto)
    {
        _totalSales = salesDto.TotalSales;
        _dailySales = salesDto.DailySales;
        _totalSalesVolumeDict = salesDto.TotalSalesVolumeDict;
        _dailySalesVolumeDict = salesDto.DailySalesVolumeDict;
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

    public void Sell(int TID, int price)
    {
        if (!_totalSalesVolumeDict.ContainsKey(TID))
        {
            _totalSalesVolumeDict.Add(TID, 0);
        }
        ++_totalSalesVolumeDict[TID];

        if (!_dailySalesVolumeDict.ContainsKey(TID))
        {
            _dailySalesVolumeDict.Add(TID, 0);
        }
        ++_dailySalesVolumeDict[TID];

        _totalSales += price;
        _dailySales += price;
    }

    public void SetSales(int totalSales, int dailySales, Dictionary<int, int> totalSalesVolumeDict, Dictionary<int, int> dailySalesVolumeDict)
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
