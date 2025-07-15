using UnityEngine;

public class Ingredient : MonoBehaviour
{
    private IngredientData _data;
    public IngredientData Data => _data;


    private void Awake()
    {

    }

    public void Init(IngredientData data)
    {
        _data = data;
    }

}
