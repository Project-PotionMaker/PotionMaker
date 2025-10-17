using System.Collections.Generic;
using UnityEngine;

public class GameSceneUIManager : MonoBehaviourSingleton<GameSceneUIManager>
{
    private Stack<GameObject> _popupStack = new();

    [SerializeField]
    private GameObject _popupNewsPaper;

    [SerializeField]
    private GameObject _popupRecipeBook;

    [SerializeField]
    private GameObject _popupMarket;

    [SerializeField]
    private GameObject _popupSettingsIngame;

    [SerializeField]
    private GameObject _popupPractice;
    public GameObject PopupPractice => _popupPractice;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnOptionEvent += OpenRecipeBookPopup;
            InputManager.Instance.OnSettingEvent += OpenSettingsIngamePopup;
            InputManager.Instance.OnCancelEvent += CloseLatestPopup;
        }
    }

    public void OpenPopup(GameObject popup)
    {
        if (popup.activeSelf)
        {
            return;
        }

        if (_popupStack.TryPeek(out GameObject currentTopPopup))
        {
            if (currentTopPopup.TryGetComponent<UI_FirstButtonSelector>(out var selector))
            {
                selector.enabled = false;
            }
        }

        _popupStack.Push(popup);
        popup.SetActive(true);
        InputManager.Instance.ChangeToUIInput();

        if (popup.TryGetComponent<UI_FirstButtonSelector>(out var newSelector))
        {
            newSelector.enabled = true;
        }
    }

    public void OpenNewspaperPopup()
    {
        OpenPopup(_popupNewsPaper);
    }

    public void OpenRecipeBookPopup()
    {
        OpenPopup(_popupRecipeBook);
    }

    public void OpenMarketPopup()
    {
        OpenPopup(_popupMarket);
    }

    public void OpenSettingsIngamePopup()
    {
        OpenPopup(_popupSettingsIngame);
    }

    public void OpenPracticePopup()
    {
        OpenPopup(_popupPractice);
    }

    public void ClosePopup(GameObject popup)
    {
        if (_popupStack.TryPeek(out GameObject latestPopup) && ReferenceEquals(latestPopup, popup))
        {
            CloseLatestPopup();
        }
    }

    private void CloseLatestPopup()
    {
        if (_popupStack.TryPop(out GameObject latestPopup))
        {
            latestPopup.SetActive(false);
            if (_popupStack.Count <= 0)
            {
                InputManager.Instance.ChangeToPlayerInput();
                return;
            }

            if (_popupStack.TryPeek(out GameObject newTopPopup))
            {
                if (newTopPopup.TryGetComponent<UI_FirstButtonSelector>(out var selector))
                {
                    selector.enabled = true;
                }
            }
        }
    }
    public void CloseAllPopups()
    {
        while (_popupStack.Count > 0)
        {
            CloseLatestPopup();
        }
    }
}
