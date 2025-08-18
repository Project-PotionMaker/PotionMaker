using System;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    public event Action<string> OnTipChanged;
    public event Action<float> OnProgressChanged;

    private float _tipChangeInterval = 5f;
    private WaitForSeconds _tipChangeWaitingCache;
    private ReadOnlyCollection<TipData> _tipDataList;

    private float _loadingDuration = 5f;
    private WaitUntil _waitUntilTipDataLoadedCache;

    private void Start()
    {
        _tipChangeWaitingCache = new WaitForSeconds(_tipChangeInterval);
        _waitUntilTipDataLoadedCache = new WaitUntil(() => DataTable.Instance.GetTipDataList() != null);
        StartCoroutine(Coroutine_ChangeTip());
        StartCoroutine(Coroutine_UpdateProgress());
    }

    private IEnumerator Coroutine_ChangeTip()
    {
        yield return _waitUntilTipDataLoadedCache;

        _tipDataList = DataTable.Instance.GetTipDataList();
        int tipDataCount = _tipDataList.Count;
        int index = UnityEngine.Random.Range(0, tipDataCount);
        while (true)
        {
            string tipText = _tipDataList[index].Description;
            index = (index + 1) % tipDataCount;
            OnTipChanged?.Invoke(tipText);
            yield return _tipChangeWaitingCache;
        }
    }

    private IEnumerator Coroutine_UpdateProgress()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _loadingDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _loadingDuration);
            OnProgressChanged?.Invoke(progress);
            yield return null;
        }
        OnProgressChanged?.Invoke(1f);
    }
}