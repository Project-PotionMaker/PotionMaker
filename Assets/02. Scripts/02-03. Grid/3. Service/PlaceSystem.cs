using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlaceSystem
{
    private List<GameObject> _placedGameObjectList = new();

    public int PlaceObject(GameObject structure, int structureTID, Vector3 position, Quaternion rotation)
    {
        structure.transform.position = position;
        structure.transform.rotation = rotation;
        _placedGameObjectList.Add(structure);

        return _placedGameObjectList.Count - 1;
    }
    public bool RemoveObjectAt(int gameObjectIndex)
    {
        if(_placedGameObjectList.Count <= gameObjectIndex
            || ReferenceEquals(_placedGameObjectList[gameObjectIndex], null))
        {
            return false;
        }
        _placedGameObjectList.RemoveAt(gameObjectIndex);
        return true;
    }

    public GameObject GetGameObject(int index)
    {
        if(index < 0 || index > _placedGameObjectList.Count)
        {
            return null;
        }
        return _placedGameObjectList[index];
    }
}
