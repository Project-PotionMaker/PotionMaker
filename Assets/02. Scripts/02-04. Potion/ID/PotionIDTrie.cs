using System;
using System.Collections.Generic;
using UnityEngine;

public class PotionIDTrie
{
    private class Node
    {
        // 숫자 0~9(10개) + 문자 A~D(4개) = 총 14개
        public Node[] children = new Node[14];
        public bool isEndOfId = false;
    }

    private Node root = new Node();

    // 0~9 → 0~9, A~D → 10~13
    private int CharToIndex(char ch)
    {
        if (ch >= '0' && ch <= '9')
        {
            return ch - '0';
        }
        if (ch >= 'A' && ch <= 'D')
        {
            return 10 + (ch - 'A');
        }
        throw new ArgumentException($"유효하지 않은 문자입니다: {ch}");
    }

    private bool IsValidFormat(string id)
    {
        if (id.Length < 6 || 8 < id.Length)
        {
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            if (id[i] < '0' || '9' < id[i])
            {
                return false;
            }
        }

        for (int i = 4; i < id.Length; i++)
        {
            if (id[i] < 'A' || '9' < id[i])
            {
                return false;
            }
        }
        return true;
    }

    public void Insert(string id)
    {
        if (!IsValidFormat(id))
        {
            Debug.LogError($"포션 ID 형식이 올바르지 않습니다 (예: 1234AB, 5678ABC): {id}");
            return;
        }
        Node current = root;

        foreach (char ch in id)
        {
            int index = CharToIndex(ch);
            if (current.children[index] == null)
            {
                current.children[index] = new Node();
            }
            current = current.children[index];
        }
        current.isEndOfId = true;
    }

    public bool Exists(string id)
    {
        if (id.Length != 8 || !IsValidFormat(id))
        {
            return false;
        }

        Node current = root;

        foreach (char ch in id)
        {
            int index = CharToIndex(ch);
            if (index < 0 || 14 <= index || current.children[index] == null)
            {
                return false;
            }
            current = current.children[index];
        }

        return current.isEndOfId;
    }

    public bool HasPrefix(string prefix)
    {
        if (prefix.Length < 1 || 8 < prefix.Length || !IsValidFormat(prefix))
        {
            return false;
        }

        Node current = root;

        foreach (char ch in prefix)
        {
            int index = CharToIndex(ch);
            if (index < 0 || index >= 14 || current.children[index] == null)
            {
                return false;
            }
            current = current.children[index];
        }

        return true;
    }

    public void PrintAll()
    {
        List<string> results = new List<string>();
        DFS(root, "", results);
        
        foreach (var id in results)
        {
            Debug.Log(id);
        }
    }

    private void DFS(Node node, string prefix, List<string> results)
    {
        if (node.isEndOfId)
        {
            results.Add(prefix);
        }

        for (int i = 0; i < 14; i++)
        {
            if (node.children[i] != null)
            {
                char ch = (i < 10) ? (char)('0' + i) : (char)('A' + (i - 10));
                DFS(node.children[i], prefix + ch, results);
            }
        }
    }
}