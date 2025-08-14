using UnityEngine;
using UnityEngine.EventSystems;

public class UI_FirstButtonSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelectedButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
