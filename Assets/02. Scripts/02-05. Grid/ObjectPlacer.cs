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
    public void RemoveObjectAt(int gameObjectIndex)
    {
        if(_placedGameObjectList.Count <= gameObjectIndex
            || ReferenceEquals(_placedGameObjectList[gameObjectIndex], null))
        {
            return;
        }
        Destroy(_placedGameObjectList[gameObjectIndex]);
        _placedGameObjectList[gameObjectIndex] = null;
    }
}
