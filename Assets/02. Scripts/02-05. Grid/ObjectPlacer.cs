using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> _placedGameObjectList = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        _placedGameObjectList.Add(newObject);

        return _placedGameObjectList.Count - 1;
    }
}
