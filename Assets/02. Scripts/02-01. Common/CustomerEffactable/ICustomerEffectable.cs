using UnityEngine;

public interface ICustomerEffectable <TStructure>
{
    public void ServerEffect(TStructure instance, Customer customer = null);
}
