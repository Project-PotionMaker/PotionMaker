using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlaceSystem

{
    private List<GameObject> _placedGameObjectList = new();

    public int PlaceObject(GameObject structure, int structureTID, Vector3 position)
    {
        structure.transform.position = position;
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
        return _placedGameObjectList[index];
    }
}
