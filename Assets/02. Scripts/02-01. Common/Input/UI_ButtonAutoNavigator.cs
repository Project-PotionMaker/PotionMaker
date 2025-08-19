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
    }

    public void AddButton(Button itemButton)
    {
        _itemButtonList.Add(itemButton);
        RefreshButtonNavigation();
    }

    private void RefreshButtonNavigation()
    {
        int maxButtonCount = _itemButtonList.Count;
        for (int i = 0; i < maxButtonCount; i++)
        {
            Navigation navigation = _itemButtonList[i].navigation;
            navigation.mode = Navigation.Mode.Explicit;

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
                navigation.selectOnUp = _itemButtonList[(line - 1) + columnIndex];
            }

            int lastLine = maxButtonCount / _maxButtonsPerRow;
            if (line != lastLine)
            {
                if (line == lastLine - 1)
                {
                    navigation.selectOnDown = _itemButtonList[(line + 1) + Mathf.Min(columnIndex,(maxButtonCount % _maxButtonsPerRow))];
                }
                else
                {
                    navigation.selectOnDown = _itemButtonList[(line + 1) + columnIndex];
                }
            }

            _itemButtonList[i].navigation = navigation;
        }

        if (_rightButton != null)
        {
            Navigation navigation = _rightButton.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnLeft = _itemButtonList[_itemButtonList.Count - 1];
            _rightButton.navigation = navigation;
        }
    }
}
