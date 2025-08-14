using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecipePage : MonoBehaviour
{
    private const string POTION_PREFIX = "Image_Potion_";
    private const string MACHINE_PREFIX = "Image_Product_";

    [Header("포션")]
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private TextMeshProUGUI _potionName;
    [SerializeField]
    private TextMeshProUGUI _potionDescription;

    [Header("재료 1")]
    [SerializeField]
    private Image _firstIngredientImage;
    [SerializeField]
    private TextMeshProUGUI _firstIngredientName;

    [Header("재료 2")]
    [SerializeField]
    private Image _secondIngredientImage;
    [SerializeField]
    private TextMeshProUGUI _secondIngredientName;

    [Header("Base Progress")]
    [SerializeField]
    private List<Image> _baseMachineList;
    [SerializeField]
    private List<Image> _baseIngredientList;

    [Header("Progress")]
    [SerializeField]
    private List<Image> _progressMachineList;
    [SerializeField]
    private List<GameObject> _arrowList;

    public async void Refresh(PotionData data)
    {
        // 포션 정보
        _potionImage.sprite = ImageManager.Instance.GetImage(typeof(PotionData), data.TID);
        _potionName.text = data.Name;
        // _potionDescription.text = data.Feature_LocalizationTID

        string recipeCode = data.RecipeCode;

        // 재료 1
        IngredientData firstIngredient = RecipeManager.Instance.IngredientDataDict[data.IngredientTIDList[0]];
        
        Sprite firstIngredientSprite = ImageManager.Instance.GetImage(typeof(IngredientData), firstIngredient.TID);
        _firstIngredientImage.sprite = firstIngredientSprite;
        _baseIngredientList[0].sprite = firstIngredientSprite;
        
        _firstIngredientName.text = firstIngredient.Name;
        _baseMachineList[0].sprite = ImageManager.Instance.GetImage(typeof(MachineData), firstIngredient.AvailableMachineTID);

        // 재료 2
        IngredientData secondIngredient = RecipeManager.Instance.IngredientDataDict[data.IngredientTIDList[1]];

        Sprite secondIngredientSprite = ImageManager.Instance.GetImage(typeof(IngredientData), secondIngredient.TID);
        _secondIngredientImage.sprite = secondIngredientSprite;
        _baseIngredientList[1].sprite = secondIngredientSprite;

        _secondIngredientName.text = secondIngredient.Name;
        _baseMachineList[1].sprite = ImageManager.Instance.GetImage(typeof(MachineData), secondIngredient.AvailableMachineTID);

        RefreshProgress(recipeCode);
    }

    private async void RefreshProgress(string recipeCode)
    {
        string machineCode = recipeCode.Substring(4);
        int progressCount = machineCode.Length - 1;

        for (int i = 0; i < _progressMachineList.Count; i++)
        {
            if (i < progressCount)
            {
                MachineData machine = RecipeManager.Instance.MachineDataDict[machineCode[i]];

                _progressMachineList[i].sprite = ImageManager.Instance.GetImage(typeof(MachineData), machine.TID);
                _progressMachineList[i].gameObject.SetActive(true);
                _arrowList[i].SetActive(true);
            }
            else
            {
                _progressMachineList[i].gameObject.SetActive(false);
                _arrowList[i].SetActive(false);
            }
        }
    }
}
