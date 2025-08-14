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

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InputManager.Instance.OnOptionEvent += OpenRecipeBookPopup;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLatestPopup();
        }
    }

    public void OpenPopup(GameObject popup)
    {
        if (popup.activeSelf)
        {
            return;
        }
        _popupStack.Push(popup);
        popup.SetActive(true);
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

    public void ClosePopup(GameObject popup)
    {
        if (_popupStack.TryPeek(out GameObject latestPopup) && ReferenceEquals(latestPopup, popup))
        {
            _popupStack.Pop();
            popup.SetActive(false);
        }
    }

    private void CloseLatestPopup()
    {
        if (_popupStack.TryPop(out GameObject latestPopup))
        {
            latestPopup.SetActive(false);
        }
    }
}
