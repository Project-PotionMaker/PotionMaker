using UnityEngine;

public class FurnitureUsingStat
{
    public bool IsUsing;
    public int HeldItemTID;
    public Customer UsingCustomer;

    public FurnitureUsingStat() 
    {
        IsUsing = false;
        HeldItemTID = 0;
        UsingCustomer = null;
    } 
}
