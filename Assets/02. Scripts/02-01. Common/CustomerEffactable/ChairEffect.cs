using Mirror;
using UnityEngine;

public class ChairEffect : ICustomerEffectable<Furniture>
{
    public void ServerEffect(Furniture furniture, NetworkIdentity customerIdentity)
    {
        Customer customer = customerIdentity.gameObject.GetComponent<Customer>();
        customer.CustomerEndurance.LoseEnduranceSpeed *= furniture.Data.EffectRate;
        customer.ChairPosition = furniture.InputPosition;
        customer.ChairRotate = furniture.CurrentRotation;
    }
}
