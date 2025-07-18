using UnityEngine;

public class RentDTO
{
    public readonly int RentPeriod;
    public readonly int RentDayCounter;
    public readonly int CurrentRentCost;
    public readonly int RentIncrement;

    public RentDTO(Rent rent)
    {
        RentPeriod = rent.RentPeriod;
        RentDayCounter = rent.RentDayCounter;
        CurrentRentCost = rent.CurrentRentCost;
        RentIncrement = rent.RentIncrement;
    }
}
