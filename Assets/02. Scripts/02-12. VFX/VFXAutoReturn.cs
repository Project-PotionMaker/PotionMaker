using Mirror;
using System.Collections;
using UnityEngine;

public class VFXAutoReturn : MonoBehaviour
{
    [SerializeField]
    private float _returnTime = 2f;

    private void OnEnable()
    {
        if (NetworkServer.active)
        {
            StartCoroutine(ReturnToFactoryWhenDone());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator ReturnToFactoryWhenDone()
    {
        yield return new WaitForSeconds(_returnTime);

        VFXFactory.Instance.ReturnObject(gameObject);
    }
}
