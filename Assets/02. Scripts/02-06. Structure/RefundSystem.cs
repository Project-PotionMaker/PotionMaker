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
        _refundable.RefundProgress = 0;
        _refundDuration = 2;
        _refundRatio = 4;
        _productPrice = DataTable.Instance.GetProductData(productTID).Price;
        _structureTID = structureTID;
    }

    public void ServerCancelRefund()
    {
        if(!ReferenceEquals(_refundRoutine, null))
        {
            RunHelper.Instance.StopCoroutine(_refundRoutine);
        }
        _refundable.RefundProgress = 0;
    }

    public void ServerStartRefund(NetworkConnectionToClient conn)
    {
        if (!ServerCanRefund() || !_isRefundable)
        {
            return;
        }
        if (!ReferenceEquals(_refundRoutine, null))
        {
            RunHelper.Instance.StopCoroutine(_refundRoutine);
        }
        _refundRoutine = RunHelper.Instance.StartCoroutine(ProcessRefund(conn));
    }

    public void ServerRefund(NetworkConnectionToClient conn)
    {
        _refundable.RefundProgress = 0;
        if (!ServerCanRefund())
        {
            OnDataChanged?.Invoke();
            return;
        }
        CurrencyManager.Instance.CmdRequestAddCurrency(_refundPrice);
        GridManager.Instance.ServerRefundStructure(_structureTID, _refundable.RefundObject);
        RefundComplete(conn);
    }

    public IEnumerator ProcessRefund(NetworkConnectionToClient conn)
    {
        while (_refundable.RefundProgress < 1)
        {
            _refundable.RefundProgress += Time.deltaTime / _refundDuration;
            yield return null;
        }
        ServerRefund(conn);
    }

    public bool ServerCanRefund()
    {
        if (GridManager.Instance.ManagedStructureDict.ContainsKey(_structureTID))
        {
            if (GridManager.Instance.ManagedStructureDict[_structureTID].Count > 1)
            {
                return true;
            }
        }

        return false;
    }

    public void RefundComplete(NetworkConnectionToClient target)
    {
        if (target.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerPickupAbility>().ReceiveRefundCompleted();
        }
    }
}
