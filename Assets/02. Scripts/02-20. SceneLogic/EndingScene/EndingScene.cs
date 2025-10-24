using System.Collections;
using UnityEngine;

public class EndingScene : MonoBehaviour
{
    [SerializeField]
    private GameObject _doorLeft;

    [SerializeField]
    private GameObject _doorRight;

    [SerializeField]
    private float _duration;

    public void OpenDoor()
    {
        StartCoroutine(Coroutine_OpenDoor());
    }

    private IEnumerator Coroutine_OpenDoor()
    {
        Quaternion leftStartRotation = _doorLeft.transform.rotation;
        Quaternion rightStartRotation = _doorRight.transform.rotation;

        Quaternion leftEndRotation = leftStartRotation * Quaternion.Euler(0, -90f, 0);
        Quaternion rightEndRotation = rightStartRotation * Quaternion.Euler(0, 90f, 0);

        float elapsedTime = 0f;
        while (elapsedTime < _duration)
        {
            float t = elapsedTime / _duration;
            _doorLeft.transform.rotation = Quaternion.Slerp(leftStartRotation, leftEndRotation, t);
            _doorRight.transform.rotation = Quaternion.Slerp(rightStartRotation, rightEndRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _doorLeft.transform.rotation = leftEndRotation;
        _doorRight.transform.rotation = rightEndRotation;
    }
}