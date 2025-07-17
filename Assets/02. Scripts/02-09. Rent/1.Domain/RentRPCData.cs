using UnityEngine;

public class RentRPCData
{
    public int RentDayCounter;
    public int CurrentRentCost;
    public int RentIncrement;

    public RentRPCData(RentDTO rentDTO)
    {
        RentDayCounter = rentDTO.RentDayCounter;
        CurrentRentCost = rentDTO.CurrentRentCost;
        RentIncrement = rentDTO.RentIncrement;
    }
}
