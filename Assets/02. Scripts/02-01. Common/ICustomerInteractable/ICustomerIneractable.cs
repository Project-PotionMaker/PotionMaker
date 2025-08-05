using UnityEngine;

public interface ICustomerInteractable
{
    public void TryCustomerPickup();

    public void TryCustomerEffect(uint customerNetId);
}
