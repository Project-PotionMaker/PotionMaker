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
        transform.forward = Camera.main.transform.forward;
    }
}
