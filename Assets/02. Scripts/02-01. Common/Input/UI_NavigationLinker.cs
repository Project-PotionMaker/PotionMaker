using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NavigationLinker : MonoBehaviour
{
    [Header("위쪽 버튼")]
    [SerializeField]
    private List<Selectable> _upSelectableList;
    [Header("아래쪽 버튼")]
    [SerializeField]
    private List<Selectable> _downSelectableList;
    [Header("왼쪽 버튼")]
    [SerializeField]
    private List<Selectable> _leftSelectableList;
    [Header("오른쪽 버튼")]
    [SerializeField]
    private List<Selectable> _rightSelectableList;

    private Selectable _selectable;

    private void Awake()
    {
        _selectable = GetComponent<Selectable>();
        Navigation navigation = _selectable.navigation;
        navigation.mode = Navigation.Mode.Explicit;

        _selectable.navigation = navigation;
    }

    private void OnEnable()
    {
        RefreshNavigation();
        RefreshLinkButtons();
    }

    public void RefreshNavigation()
    {
        if (_selectable == null)
        {
            return;
        }

        Navigation navigation = _selectable.navigation;

        navigation.selectOnUp = FindFirstActiveSelectable(_upSelectableList);
        navigation.selectOnDown = FindFirstActiveSelectable(_downSelectableList);
        navigation.selectOnLeft = FindFirstActiveSelectable(_leftSelectableList);
        navigation.selectOnRight = FindFirstActiveSelectable(_rightSelectableList);

        _selectable.navigation = navigation;
    }

    private void RefreshLinkButtons()
    {
        Navigation navigation = _selectable.navigation;
        NotifyNeighbor(navigation.selectOnUp);
        NotifyNeighbor(navigation.selectOnDown);
        NotifyNeighbor(navigation.selectOnLeft);
        NotifyNeighbor(navigation.selectOnRight);
    }

    private void NotifyNeighbor(Selectable neighbor)
    {
        if (neighbor == null)
        {
            return;
        }

        if (neighbor.TryGetComponent<UI_NavigationLinker>(out var neighborLinker))
        {
            neighborLinker.RefreshNavigation();
        }
    }

    private Selectable FindFirstActiveSelectable(List<Selectable> targetSelectableList)
    {
        foreach (Selectable target in targetSelectableList)
        {
            if (target != null && target.gameObject.activeInHierarchy
                && target.interactable)
            {
                return target;
            }
        }

        return null;
    }
}
