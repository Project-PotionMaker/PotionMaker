using UnityEngine;

public class ChairEffect : ICustomerEffectable<Furniture, FurnitureStat>
{
    public void Effect(Furniture furniture, FurnitureStat stat, Customer customer)
    {
        customer.CustomerEndurance.LoseEnduranceSpeed *= stat.Data.EffectRate;
        customer.ChairPosition = stat.InputPosition;
        customer.ChairRotate = stat.CurrentRotation;
    }
}
