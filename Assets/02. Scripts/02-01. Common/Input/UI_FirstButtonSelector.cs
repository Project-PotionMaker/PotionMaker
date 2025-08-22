using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_FirstButtonSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelectedButton;
    private GameObject _lastSelectedButton;

    private void OnEnable()
    {
        StartCoroutine(SubscribeNextFrame());
    }

    private IEnumerator SubscribeNextFrame()
    {
        yield return null;

        _lastSelectedButton = null;
        InputManager.Instance.OnNavigateEvent += ChangeNavigationMode;
        InputManager.Instance.OnPointEvent += ChangePointerMode;

        if (_firstSelectedButton != null && _firstSelectedButton.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
        }
    }

    private void OnDisable()
    {
        _lastSelectedButton = null;
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnNavigateEvent -= ChangeNavigationMode;
            InputManager.Instance.OnPointEvent -= ChangePointerMode;
        }
    }

    public void ChangeSelected(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(target);
    }

    private void ChangeNavigationMode()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }

        if (_lastSelectedButton == null || !_lastSelectedButton.gameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
            _lastSelectedButton = _firstSelectedButton;
            return;
        }

        EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
    }

    private void ChangePointerMode()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected == null)
        {
            return;
        }

        _lastSelectedButton = currentSelected;

        if (currentSelected.GetComponent<TMP_InputField>() != null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}
