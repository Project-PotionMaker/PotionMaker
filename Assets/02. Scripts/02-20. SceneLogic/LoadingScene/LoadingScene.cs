using System;
using System.Collections;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    public event Action<string> OnTipChanged;

    private float _tipChangePeriod = 5f;
    private WaitForSeconds _tipChangeWaitingCache;

    private ReadOnlyList<TipData> _tipDataList;

    private void Awake()
    {
    }

    private void Start()
    {
        _tipChangeWaitingCache = new WaitForSeconds(_tipChangePeriod);
        Global.Instance.OnDataLoaded += () =>
        {
            _tipDataList = DataTable.Instance.GetTipDataList();
            StartCoroutine(Coroutine_ChangeTip());
        };
    }

    


    private IEnumerator Coroutine_ChangeTip()
    {
        int index = 0;
        int tipDataCount = _tipDataList.Count;
        while (true)
        {
            string tipText = _tipDataList[index].Description;
            index = (index + 1) % tipDataCount;
            OnTipChanged?.Invoke(tipText);
            yield return _tipChangeWaitingCache;
        }
    }
}
