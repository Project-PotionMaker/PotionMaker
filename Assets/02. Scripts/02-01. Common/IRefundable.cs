using UnityEngine;

public interface IRefundable
{
    public float RefundGauge { get; set; }
    public abstract void AddRefundGauge();
    public abstract void ResetRefundGauge();
    public abstract void Refund();
}
