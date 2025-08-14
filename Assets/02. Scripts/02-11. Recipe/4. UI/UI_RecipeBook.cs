using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecipeBook : MonoBehaviour
{
    [Header("RecipePage")]
    [SerializeField]
    private UI_RecipePage _leftRecipePage;
    [SerializeField]
    private UI_RecipePage _rightRecipePage;
    [SerializeField]
    private GameObject _emptyPage;

    [Header("티어 이동 버튼")]
    [SerializeField]
    private List<Button> _tierButtonList;
    [SerializeField]
    private Transform _selectedButtonParent;
    [SerializeField]
    private Transform _unselectedButtonParent;
    private ReadOnlyList<int> _currentPotionTIDList;
    private ETierType _currentTier = ETierType.Tier1;

    [Header("페이지 이동 버튼")]
    [SerializeField]
    private Button _prevButton;
    [SerializeField]
    private Button _nextButton;
    private int _currentLeftPage = 0;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    { 
        for (int i = 0; i < _tierButtonList.Count; i++)
        {
            int index = i + 1;
            _tierButtonList[i].onClick.AddListener(() => ChangeTier(index));

            if (i + 1 <= PotionHouse.Instance.PotionHouseTier)
            {
                _tierButtonList[i].interactable = true;
            }
            else
            {
                _tierButtonList[i].interactable = false;
            }
        }

        _currentTier = ETierType.Tier1;
        ChangeTier(1);
    }

    public void ChangeTier(int tier)
    {
        if (tier > PotionHouse.Instance.PotionHouseTier)
        {
            return;
        }

        int prevTier = (int)(_currentTier + 1);

        _tierButtonList[prevTier - 1].transform.SetParent(_unselectedButtonParent);
        _tierButtonList[tier - 1].transform.SetParent(_selectedButtonParent);

        _currentTier = (ETierType)(tier - 1);
        _currentPotionTIDList = PotionHouse.Instance.UnlockedPotionTierDict[tier];
        _currentLeftPage = 0;
        RefreshPage();
    }

    private void RefreshPage()
    {
        if (_currentLeftPage >= _currentPotionTIDList.Count)
        {
            return;
        }

        int potionTID = _currentPotionTIDList[_currentLeftPage];
        PotionData leftPotionData = RecipeManager.Instance.PotionDataDict[potionTID];
        _leftRecipePage.Refresh(leftPotionData);

        int rightPage = _currentLeftPage + 1;
        if (rightPage >= _currentPotionTIDList.Count)
        {
            _emptyPage.SetActive(true);
            return;
        }

        _emptyPage.SetActive(false);

        potionTID = _currentPotionTIDList[rightPage];
        PotionData rightPotionData = RecipeManager.Instance.PotionDataDict[potionTID];
        _rightRecipePage.Refresh(rightPotionData);
    }

    public void OnClickNextPageButton()
    {
        if (_currentLeftPage + 2 >= _currentPotionTIDList.Count)
        {
            return;
        }

        _currentLeftPage += 2;

        RefreshPage();
    }

    public void OnClickPrevPageButton()
    {
        if (_currentLeftPage - 2 < 0)
        {
            return;
        }

        _currentLeftPage -= 2;

        RefreshPage();
    }
}
