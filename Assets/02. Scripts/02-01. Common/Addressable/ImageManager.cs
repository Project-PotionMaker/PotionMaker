using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviourSingleton<ImageManager>
{
    private Dictionary<Type, string> _prefixDict = new();
    private Dictionary<Type, Dictionary<int, Sprite>> _imageDict = new();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Global.Instance.OnDataLoaded += InitImageManager;
    }

    private void OnDestroy()
    {
        if (Global.Instance != null)
        {
            Global.Instance.OnDataLoaded -= InitImageManager;
        }
    }

    private async void InitImageManager()
    {
        InitPrefixDict();
        // await InitImageDict<IngredientData>(DataTable.Instance.GetIngredientDataList());
        await InitImageDict<PotionData>(DataTable.Instance.GetPotionDataList());
        await InitImageDict<ProductData>(DataTable.Instance.GetProductDataList());
    }

    private void InitPrefixDict()
    {
        _prefixDict[typeof(IngredientData)] = "Image_Ingredient_";
        _prefixDict[typeof(PotionData)] = "Image_Potion_";
        _prefixDict[typeof(ProductData)] = "Image_Product_";
    }

    private async Task InitImageDict<T>(ReadOnlyList<T> dataList)
    {
        var type = typeof(T);
        if (!_prefixDict.TryGetValue(type, out string prefix))
        {
            Debug.LogWarning($"{type.Name} 타입은 이미지 매니저에서 캐싱을 통해 관리하는 대상이 아닙니다.");
            return;
        }

        var tidField = type.GetField("TID");
        if (tidField == null || tidField.FieldType != typeof(int))
        {
            Debug.LogWarning($"{type.Name} 타입은 TID를 가지고 있지 않습니다.");
            return;
        }

        Dictionary<int, Sprite> dict = new();


        var loadTasks = dataList.Select(item => LoadImageForItem(item, prefix, tidField));
        var results = await Task.WhenAll(loadTasks);
        foreach (var result in results)
        {
            if (result.HasValue)
            {
                dict[result.Value.tid] = result.Value.sprite;
            }
        }
        _imageDict[type] = dict;
    }

    private async Task<(int tid, Sprite sprite)?> LoadImageForItem<T>(T item, string prefix, FieldInfo tidField)
    {
        int tid = (int)tidField.GetValue(item);
        string addressableKey = prefix + tid;

        Sprite sprite = await AssetManager.Instance.LoadAsset<Sprite>(addressableKey);
        if (sprite == null)
        {
            Debug.LogWarning($"{addressableKey} 주소를 가진 이미지가 Addressable에 존재하지 않습니다.");
            return null;
        }
        return (tid, sprite);
    }

    public Sprite GetImage(Type type, int id)
    {
        if (_imageDict.TryGetValue(type, out var dict) && dict.TryGetValue(id, out var sprite))
        {
            return sprite;
        }

        Debug.LogWarning($"{type.Name} 타입의 {id} TID를 가진 이미지 리소스가 존재하지 않습니다. " +
            $"어드레서블 그룹을 다시한번 확인해주세요");
        return null;
    }

    public Sprite GetImage<T>(int id)
    {
        var type = typeof(T);
        return GetImage(type, id);
    }

    public Dictionary<int, Sprite> GetImageDict(Type type)
    {
        if (_imageDict.TryGetValue(type, out var dict))
        {
            return dict;
        }
        return null;
    }

    public Dictionary<int, Sprite> GetImageDict<T>()
    {
        var type = typeof(T);
        return GetImageDict(type);
    }
}
