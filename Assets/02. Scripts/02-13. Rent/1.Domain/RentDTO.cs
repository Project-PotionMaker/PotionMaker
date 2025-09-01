using System;
using UnityEngine;

[Serializable]
public class RentDTO
{
    public readonly int RentPeriod;
    public readonly int RentDayCounter;
    public readonly int CurrentRentCost;
    public readonly int LastRentCost;
    public readonly int RentIncrement;
    public bool IsRentDay => RentDayCounter == RentPeriod;

    public RentDTO(Rent rent)
    {
        RentPeriod = rent.RentPeriod;
        RentDayCounter = rent.RentDayCounter;
        CurrentRentCost = rent.CurrentRentCost;
        LastRentCost = rent.LastRentCost;
        RentIncrement = rent.RentIncrement;
    }
}
