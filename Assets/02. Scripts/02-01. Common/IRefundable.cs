using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface IRefundable
{
    public float RefundProgress { get; set; }
    public GameObject RefundObject { get; }
    public abstract void StartRefund();
    public abstract void CancelRefund();
}
