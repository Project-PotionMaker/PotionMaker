using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridTest_InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera _sceneCamera;
    [SerializeField]
    private LayerMask _placementLayerMask;

    private Vector3 _lastPosition;

    public Action OnClicked;
    public Action OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, _placementLayerMask))
        {
            _lastPosition = hit.point;
        }
        return _lastPosition;
    }

}
