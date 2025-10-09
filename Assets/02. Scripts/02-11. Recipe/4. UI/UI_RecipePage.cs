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

    public void Refresh(PotionData data)
    {
        RefreshPotionBasicInfo(data);
        RefreshPotionIngredientInfo(data, 0);
        RefreshPotionIngredientInfo(data, 1);
        RefreshProgressInfo(data.RecipeCode);
    }

    // 포션 이미지, 이름, 설명
    private void RefreshPotionBasicInfo(PotionData data)
    {
        _potionImage.sprite = ImageManager.Instance.GetImage(typeof(PotionData), data.TID);
        _potionName.text = data.Name;
        _potionDescription.text = data.TestDescription;
    }

    // 재료 이미지, 이름, 사용 가능한 머신 이미지
    private void RefreshPotionIngredientInfo(PotionData data, int ingredientIndex)
    {
        IngredientData ingredientData = RecipeManager.Instance.IngredientDataDict[data.IngredientTIDList[ingredientIndex]];

        Sprite firstIngredientSprite = ImageManager.Instance.GetImage(typeof(IngredientData), ingredientData.TID);
        _firstIngredientImage.sprite = firstIngredientSprite;
        _baseIngredientList[ingredientIndex].sprite = firstIngredientSprite;

        _firstIngredientName.text = ingredientData.Name;
        _baseMachineList[ingredientIndex].sprite = ImageManager.Instance.GetImage(typeof(MachineData), ingredientData.AvailableMachineTID);
    }

    // 제작 프로세스
    private void RefreshProgressInfo(string recipeCode)
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
