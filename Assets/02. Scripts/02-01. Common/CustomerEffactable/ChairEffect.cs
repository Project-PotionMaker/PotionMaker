using Mirror;
using UnityEngine;

public class ChairEffect : ICustomerEffectable<Furniture>
{
    public void ServerEffect(Furniture furniture, NetworkIdentity customerIdentity)
    {
        //customer.CustomerEndurance.LoseEnduranceSpeed *= stat.Data.EffectRate;
        //customer.ChairPosition = stat.InputPosition;
        //customer.ChairRotate = stat.CurrentRotation;
    }
}
