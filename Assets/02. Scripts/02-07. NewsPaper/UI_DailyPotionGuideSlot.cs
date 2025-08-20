using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DailyPotionGuideSlot : MonoBehaviour
{
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private Image _firstIngredientImage;
    [SerializeField]
    private Image _secondIngredientImage;

    public void Refresh(PotionData potionData)
    {
        StartCoroutine(Refresh_Coroutine(potionData));
    }

    private IEnumerator Refresh_Coroutine(PotionData potionData)
    {
        while(RecipeManager.Instance.IngredientDataDict.Count == 0)
        {
            Debug.Log("ZZ");
            yield return new WaitForSeconds(0.05f);
        }

        // 포션 이미지
        _potionImage.sprite = ImageManager.Instance.GetImage<PotionData>(potionData.TID);

        // 재료1 이미지
        IngredientData firstIngredient = RecipeManager.Instance.IngredientDataDict[potionData.IngredientTIDList[0]];
        Debug.Log($"재료1: {firstIngredient.TID}");
        Sprite firstIngredientSprite = ImageManager.Instance.GetImage(typeof(IngredientData), firstIngredient.TID);
        _firstIngredientImage.sprite = firstIngredientSprite;

        // 재료2 이미지
        IngredientData secondIngredient = RecipeManager.Instance.IngredientDataDict[potionData.IngredientTIDList[1]];
        Debug.Log($"재료2: {secondIngredient.TID}");
        Sprite secondIngredientSprite = ImageManager.Instance.GetImage(typeof(IngredientData), secondIngredient.TID);
        _secondIngredientImage.sprite = secondIngredientSprite;
    }
}
