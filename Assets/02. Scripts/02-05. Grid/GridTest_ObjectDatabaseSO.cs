using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GridTest_ObjectDatabaseSO", menuName = "Scriptable Objects/GridTest_ObjectDatabaseSO")]
public class GridTest_ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> ObjectsDataList;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
