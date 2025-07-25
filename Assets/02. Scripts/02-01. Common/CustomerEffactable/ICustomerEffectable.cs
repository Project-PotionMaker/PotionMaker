using UnityEngine;

public interface ICustomerEffectable <TClass,TStat>
{
    public void Effect(TClass instance, TStat stat, Customer customer = null);
}
