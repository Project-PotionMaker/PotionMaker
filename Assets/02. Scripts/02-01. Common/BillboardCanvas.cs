using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera == null)
            return;

        // 카메라를 바라보게 회전
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
    }
}
