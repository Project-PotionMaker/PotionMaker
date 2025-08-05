using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface IRefundable
{
    public abstract void StartRefund();
    public abstract void CancelRefund();
    public abstract void Refund();
}
