using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlaceSystem

{
    private List<GameObject> _placedGameObjectList = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        Debug.Log("인스턴스화 포톤네트워크에서 필요");
        GameObject newObject = GameObject.Instantiate(prefab);
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
        GameObject.Destroy(_placedGameObjectList[gameObjectIndex]);
        _placedGameObjectList[gameObjectIndex] = null;
    }
}
