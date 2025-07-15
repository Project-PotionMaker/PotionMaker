using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCodeVerifier : MonoBehaviour
{
    private RecipeCodeTrie _potionIdTrie;
    public RecipeCodeTrie PotionIdTrie => _potionIdTrie;

    private void Awake()
    {
        _potionIdTrie = new RecipeCodeTrie();
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
