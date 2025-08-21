using UnityEngine;

public class RentRPCData
{
    public int RentDayCounter;
    public int CurrentRentCost;
    public int LastRentCost;
    public int RentIncrement;

    public RentRPCData(RentDTO rentDTO)
    {
        RentDayCounter = rentDTO.RentDayCounter;
        CurrentRentCost = rentDTO.CurrentRentCost;
        LastRentCost = rentDTO.LastRentCost;
        RentIncrement = rentDTO.RentIncrement;
    }
}
