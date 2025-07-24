using UnityEngine;

public class PickingStat
{
    public bool IsPotionExist;
    public int potionTID;
    public Customer Picker;

    public PickingStat() 
    {
        IsPotionExist = false;
        potionTID = 0;
        Picker = null;
    } 
}
