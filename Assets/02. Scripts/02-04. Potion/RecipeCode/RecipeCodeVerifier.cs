using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCodeVerifier
{
    private RecipeCodeTrie _potionIdTrie;
    public RecipeCodeTrie PotionIdTrie => _potionIdTrie;

    public RecipeCodeVerifier(ReadOnlyList<PotionData> potionDataList)
    {
        _potionIdTrie = new RecipeCodeTrie();
        Init(potionDataList);
    }

    private void Init(ReadOnlyList<PotionData> potionDataList)
    {
        foreach (var potionData in potionDataList)
        {
            PotionIdTrie.Insert(potionData.RecipeCode);
        }
    }

    public bool IsValidProcess(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("ID가 null이거나 비어 있습니다.");
            return false;
        }
        if (_potionIdTrie.HasPrefix(id))
        {
            return true;
        }

        return false;
    }

    public bool IsValidPotion(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("ID가 null이거나 비어 있습니다.");
            return false;
        }

        if (_potionIdTrie.Exists(id))
        {
            return true;
        }
        return false;
    }
}
