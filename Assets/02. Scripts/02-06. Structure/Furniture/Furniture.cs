using UnityEngine;

public class Furniture : MonoBehaviour
{
    private FurnitureData _data;
    public FurnitureData Data => _data;

    public void Init(FurnitureData data)
    {
        _data = data;
    }
}
