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

    private void Start()
    {
        _visibilityIndex = 0;
        Refresh();
    }

    public void OnClickLeftButton()
    {
        _visibilityIndex = (_visibilityIndex - 1 + Enum.GetNames(typeof(Visibility)).Length)
                           % Enum.GetNames(typeof(Visibility)).Length;
        Refresh();
    }

    public void OnClickRightButton()
    {
        _visibilityIndex = (_visibilityIndex + 1) % Enum.GetNames(typeof(Visibility)).Length;
        Refresh();
    }

    private void Refresh()
    {
        _textVisibilityOption.text = ((Visibility)_visibilityIndex).ToString();
        _roomInfoHandler.UpdateRoomInfo((Visibility)_visibilityIndex);
    }
}
