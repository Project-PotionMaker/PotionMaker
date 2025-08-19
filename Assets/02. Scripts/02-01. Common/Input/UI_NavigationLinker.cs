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

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        Navigation navigation = _button.navigation;
        navigation.mode = Navigation.Mode.Explicit;

        _button.navigation = navigation;
    }

    private void OnEnable()
    {
        RefreshNavigation();
        RefreshLinkButtons();
    }

    public void RefreshNavigation()
    {
        if (_button == null)
        {
            return;
        }

        Navigation navigation = _button.navigation;

        navigation.selectOnUp = FindFirstActiveSelectable(_upSelectableList);
        navigation.selectOnDown = FindFirstActiveSelectable(_downSelectableList);
        navigation.selectOnLeft = FindFirstActiveSelectable(_leftSelectableList);
        navigation.selectOnRight = FindFirstActiveSelectable(_rightSelectableList);

        _button.navigation = navigation;
    }

    private void RefreshLinkButtons()
    {
        Navigation navigation = _button.navigation;
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
            if (target != null && target.gameObject.activeInHierarchy)
            {
                return target;
            }
        }

        return null;
    }
}
