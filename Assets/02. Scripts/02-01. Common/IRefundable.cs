using System.Collections;
using UnityEngine;

public interface IRefundable
{
    public float RefundGauge { get; set; }
    public Coroutine RefundRoutine { get; set; }
    public abstract void StartRefund();
    public abstract void CancelRefund();
    public abstract void Refund();

    public abstract IEnumerator ProcessRefund(); 
}
