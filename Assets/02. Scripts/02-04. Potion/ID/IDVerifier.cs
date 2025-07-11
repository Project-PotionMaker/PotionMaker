using System;
using System.Collections.Generic;
using UnityEngine;

public class IDVerifier : MonoBehaviour
{
    private PotionIDTrie _potionIdTrie;
    public PotionIDTrie PotionIdTrie => _potionIdTrie;

    private void Awake()
    {
        _potionIdTrie = new PotionIDTrie();
    }

    public bool IsValidProcess(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("ID�� null�̰ų� ��� �ֽ��ϴ�.");
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
            Debug.LogWarning("ID�� null�̰ų� ��� �ֽ��ϴ�.");
            return false;
        }

        if (_potionIdTrie.Exists(id))
        {
            return true;
        }
        return false;
    }
}
