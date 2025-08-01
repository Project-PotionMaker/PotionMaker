using System;
using UnityEngine;
using TMPro;

public class UI_Visibility : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField]
    private RoomInfoHandler _roomInfoHandler;

    [SerializeField]
    private TextMeshProUGUI _textVisibilityOption;

    private int _visibilityIndex;
    private int _visibilityEnumSize;

    private void Start()
    {
        _visibilityIndex = 0;
        _visibilityEnumSize = Enum.GetNames(typeof(Visibility)).Length;
        Refresh();
    }

    public void OnClickLeftButton()
    {
        _visibilityIndex = (_visibilityIndex - 1 + _visibilityEnumSize) % _visibilityEnumSize;
        Refresh();
    }

    public void OnClickRightButton()
    {
        _visibilityIndex = (_visibilityIndex + 1) % _visibilityEnumSize;
        Refresh();
    }

    private void Refresh()
    {
        _textVisibilityOption.text = ((Visibility)_visibilityIndex).ToString();
        _roomInfoHandler.UpdateRoomInfo((Visibility)_visibilityIndex);
    }
}
