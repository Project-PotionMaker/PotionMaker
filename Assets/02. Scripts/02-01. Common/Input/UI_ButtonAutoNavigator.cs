using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonAutoNavigator : MonoBehaviour
{
    [Header("새로운 창 버튼")]
    [SerializeField]
    private List<Button> _categoryButtonList;
    [Header("닫기 버튼")]
    [SerializeField]
    private Button _exitButton;

    [Header("한 줄의 최대 버튼 개수")]
    [SerializeField]
    private int _maxButtonsPerRow = 4;
    private List<Button> _itemButtonList = new List<Button>();

    [Header("버튼들 오른쪽 버튼")]
    [SerializeField]
    private Button _rightButton;

    private void Start()
    {
        int categoryCount = _categoryButtonList.Count;
        for (int i = 0; i < categoryCount; i++)
        {
            Navigation navigation = _categoryButtonList[i].navigation;
            navigation.mode = Navigation.Mode.Explicit;

            if (i < categoryCount - 1)
            {
                navigation.selectOnRight = _categoryButtonList[i + 1];
            }
            else
            {
                navigation.selectOnRight = _exitButton;
            }

            if (i > 0)
            {
                navigation.selectOnLeft = _categoryButtonList[i - 1];
            }

            _categoryButtonList[i].navigation = navigation;
        }

        if (_exitButton != null)
        {
            Navigation navigation = _exitButton.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnDown = _rightButton;
            navigation.selectOnLeft = _categoryButtonList[_categoryButtonList.Count - 1];
            _exitButton.navigation = navigation;
        }

        if (_rightButton != null)
        {
            Navigation navigation = _rightButton.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = _exitButton;
            _rightButton.navigation = navigation;
        }
    }

    public void RefreshButtonList(List<Button> buttonList)
    {
        _itemButtonList = buttonList;
        RefreshButtonNavigation();
    }

    private void RefreshButtonNavigation()
    {
        int maxButtonCount = _itemButtonList.Count;
        int lastEnableButton = 0;
        bool isAllEnabled = true;
        for (int i = 0; i < maxButtonCount; i++)
        {
            Navigation navigation = _itemButtonList[i].navigation;
            navigation.mode = Navigation.Mode.Explicit;

            if (isAllEnabled && !_itemButtonList[i].enabled)
            {
                isAllEnabled = false;
                lastEnableButton = i - 1;
                Navigation lastButtonNavigation = _itemButtonList[lastEnableButton].navigation;
                lastButtonNavigation.selectOnRight = _rightButton;
                _itemButtonList[lastEnableButton].navigation = lastButtonNavigation;
                break;
            }

            if (i < maxButtonCount - 1)
            {
                navigation.selectOnRight = _itemButtonList[i + 1];
            }
            if (i > 0)
            {
                navigation.selectOnLeft = _itemButtonList[i - 1];
            }

            int line = i / _maxButtonsPerRow;
            int columnIndex = i % _maxButtonsPerRow;

            if ((_rightButton != null) && (columnIndex == _maxButtonsPerRow - 1))
            {
                navigation.selectOnRight = _rightButton;
            }

            if (line == 0)
            {
                navigation.selectOnUp = _categoryButtonList[Mathf.Min(i, _categoryButtonList.Count - 1)];
            }
            else
            {
                navigation.selectOnUp = _itemButtonList[(line - 1) * _maxButtonsPerRow + columnIndex];
            }

            int downIndex = i + _maxButtonsPerRow;
            if (downIndex < maxButtonCount)
            {
                navigation.selectOnDown = _itemButtonList[downIndex];
            }
            else
            {
                int lastLineIndex = (maxButtonCount - 1) / _maxButtonsPerRow;
                if (line < lastLineIndex)
                {
                    int targetIndex = lastLineIndex * _maxButtonsPerRow + columnIndex;
                    navigation.selectOnDown = _itemButtonList[Mathf.Min(targetIndex, maxButtonCount - 1)];
                }
            }

            _itemButtonList[i].navigation = navigation;
        }


        for (int i = 0; i < _categoryButtonList.Count; i++)
        {
            Navigation navigation = _categoryButtonList[i].navigation;
            navigation.selectOnDown = _itemButtonList[Mathf.Min(i, _itemButtonList.Count)];
            _categoryButtonList[i].navigation = navigation;
        }

        if (_rightButton != null && _itemButtonList.Count > 0)
        {
            Navigation navigation = _rightButton.navigation;
            if (isAllEnabled)
            {
                navigation.selectOnLeft = _itemButtonList[_itemButtonList.Count - 1];
            }
            else
            {
                navigation.selectOnLeft = _itemButtonList[lastEnableButton];
            }

            _rightButton.navigation = navigation;
        }
    }
}
