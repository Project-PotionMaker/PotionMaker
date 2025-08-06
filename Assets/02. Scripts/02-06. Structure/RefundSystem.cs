using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class RefundSystem
{
    public event Action OnDataChanged;

    private IRefundable _refundable;
    private bool _isRefundable;
    private float _refundDuration;
    private int _refundRatio;
    private int _productPrice;
    private int _refundPrice => _productPrice / _refundRatio;
    private int _structureTID;
    private Coroutine _refundRoutine;

    public void InitRefundSyStem(int structureTID, IRefundable refundable)
    {
        int productTID = DataTable.Instance.GetStructureData(structureTID).ProductTID;
        if (productTID < 0)
        {
            _isRefundable = false;
            return;
        }
        _isRefundable = true;
        _refundable = refundable;
        _refundDuration = 2;
        _refundRatio = 4;
        _productPrice = DataTable.Instance.GetProductData(productTID).Price;
        _structureTID = structureTID;
    }

    public void CancelRefund()
    {
        if(!ReferenceEquals(_refundRoutine, null))
        {
            RunHelper.Instance.StopCoroutine(_refundRoutine);
        }
        _refundable.RefundProgress = 0;
    }

    public void StartRefund()
    {
        if (!CanRefund() || !_isRefundable)
        {
            return;
        }
        if (!ReferenceEquals(_refundRoutine, null))
        {
            RunHelper.Instance.StopCoroutine(_refundRoutine);
        }
        _refundRoutine = RunHelper.Instance.StartCoroutine(ProcessRefund());
    }

    public void Refund()
    {
        if (!CanRefund())
        {
            _refundable.RefundProgress = 0;
            OnDataChanged?.Invoke();
            return;
        }
        CurrencyManager.Instance.CmdRequestAddCurrency(_refundPrice);
        StructureFactory.Instance.ReturnObject(_refundable.RefundObject);
    }

    public IEnumerator ProcessRefund()
    {
        while (_refundable.RefundProgress < 1)
        {
            _refundable.RefundProgress += Time.deltaTime / _refundDuration;
            yield return null;
        }
        Refund();
    }

    public bool CanRefund()
    {
        ReadOnlyList<int> structureList = GridManager.Instance.GetPlacedStructureTIDList();
        foreach(int structureTID in structureList)
        {
            if(structureTID == _structureTID)
            {
                return true;
            }
        }
        return false;
    }
}
