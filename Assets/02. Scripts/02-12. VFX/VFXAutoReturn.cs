using System.Collections;
using UnityEngine;

public class VFXAutoReturn : MonoBehaviour
{
    [SerializeField]
    private float _returnTime = 2f;

    private void OnEnable()
    {
        StartCoroutine(ReturnToFactoryWhenDone());
    }

    private IEnumerator ReturnToFactoryWhenDone()
    {
        yield return new WaitForSeconds(_returnTime);

        VFXFactory.Instance.ReturnObject(gameObject);
    }
}
