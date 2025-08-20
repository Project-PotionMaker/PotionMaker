using System.Collections.Generic;
using UnityEngine;

public class LobbySceneUIManager : MonoBehaviourSingleton<LobbySceneUIManager>
{
    private Stack<GameObject> _popupStack = new Stack<GameObject>();

    public void RegisterSelector(GameObject popup)
    {
        if (_popupStack.TryPeek(out var topPopup))
        {
            if (topPopup.TryGetComponent<UI_FirstButtonSelector>(out var currentTopselector))
            {
                currentTopselector.enabled = false;
            }
        }

        _popupStack.Push(popup);

        if (popup.TryGetComponent<UI_FirstButtonSelector>(out var newSelector))
        {
            newSelector.enabled = true;
        }
    }

    public void PopPopup()
    {
        if (_popupStack.Count == 0) return;

        // 현재 팝업을 스택에서 제거하고 비활성화
        GameObject popupToClose = _popupStack.Pop();
        if (popupToClose != null && popupToClose.TryGetComponent<UI_FirstButtonSelector>(out var selectorToClose))
        {
            selectorToClose.enabled = false;
        }

        // 스택에 남아있는 이전 팝업이 있다면 활성화
        if (_popupStack.TryPeek(out var nextPopup))
        {
            if (nextPopup != null && nextPopup.TryGetComponent<UI_FirstButtonSelector>(out var selectorToEnable))
            {
                selectorToEnable.enabled = true;
            }
        }
    }
}
