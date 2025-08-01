using Mirror;
using UnityEngine;

public interface ICustomerEffectable <TStructure>
{
    public void ServerEffect(TStructure instance, NetworkIdentity customerIdentity = null);
}
