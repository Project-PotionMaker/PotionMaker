using System;
using System.Collections.Generic;
using UnityEngine;

public class HallAreaPathFinder
{
    [Header("Depend on Layout")]
    private HashSet<Vector3Int> _hallAreaPositionHashSet = new(); // Init 이후 불변
    private HashSet<Vector3Int> _placedPositionHashSet = new(); // 가변
    private HashSet<Vector3Int> _chairPositionHashSet = new(); // 가변
    private HashSet<Vector3Int> _pickupTablePositionHashSet = new(); // 가변
    private Vector3Int _cashierPosition = new(); // Init 이후 불변
    private Vector3Int _entrancePosition = new(); // Init 이후 불변
    private Vector3Int _exitPosition = new(); // Init 이후 불변

    private readonly Vector3Int[] _directions =
    {
        new(0, 0, 1),
        new(0, 0, -1),
        new(-1, 0, 0),
        new(1, 0, 0)
    };

    private Queue<Vector3Int> _queue = new();
    private HashSet<Vector3Int> _visited = new();

    public HallAreaPathFinder() {}

    public void InitGridPathFinder(
        HashSet<Vector3Int> gridPositionHashSet,
        HashSet<Vector3Int> placedPositionHashSet,
        Vector3Int cashierPosition,
        Vector3Int entrancePosition,
        Vector3Int exitPosition,
        HashSet<Vector3Int> pickupTablePositionHashSet)
    {
        _hallAreaPositionHashSet = gridPositionHashSet;
        _placedPositionHashSet = placedPositionHashSet;
        _cashierPosition = cashierPosition;
        _entrancePosition = entrancePosition;
        _exitPosition = exitPosition;
        _pickupTablePositionHashSet = pickupTablePositionHashSet;
        MakeChairPositionHashSet();
    }

    public void UpdateGridPathFinder(HashSet<Vector3Int> placedPositionHashSet, HashSet<Vector3Int> pickupTablePositionHashSet)
    {
        _placedPositionHashSet = placedPositionHashSet;
        _pickupTablePositionHashSet = pickupTablePositionHashSet;
        MakeChairPositionHashSet();
    }

    private void MakeChairPositionHashSet()
    {
        _chairPositionHashSet.Clear();
        foreach (var pos in _placedPositionHashSet)
        {
            if (_hallAreaPositionHashSet.Contains(pos))
            {
                _chairPositionHashSet.Add(pos);
            }
        }
    }

    public bool HasPath()
    {
        if (_placedPositionHashSet.Contains(_entrancePosition) 
            || _placedPositionHashSet.Contains(_exitPosition)
            || _pickupTablePositionHashSet.Count < 1)
        {
            return false;
        }

        HashSet<Vector3Int> reachablePositionHashSet = GetReachablePositionsFrom(_entrancePosition);
        return reachablePositionHashSet.Contains(_cashierPosition) 
            && reachablePositionHashSet.Contains(_exitPosition) 
            && reachablePositionHashSet.IsSupersetOf(_chairPositionHashSet) 
            && reachablePositionHashSet.IsSupersetOf(_pickupTablePositionHashSet);
    }

    private HashSet<Vector3Int> GetReachablePositionsFrom(Vector3Int start)
    {
        InitQueueAndVisited(start);
        HashSet<Vector3Int> reachablePositionHashSet = new() { start };
        while (_queue.TryDequeue(out Vector3Int currentPosition))
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3Int nextPosition = currentPosition + _directions[i];  
                if (IsPositionValid(nextPosition))
                {
                    _queue.Enqueue(nextPosition);
                    _visited.Add(nextPosition);
                    reachablePositionHashSet.Add(nextPosition);
                }
                else if (_pickupTablePositionHashSet.Contains(nextPosition) 
                    || _chairPositionHashSet.Contains(nextPosition) 
                    || nextPosition == _cashierPosition)
                {
                    reachablePositionHashSet.Add(nextPosition);
                }
            }
        }
        return reachablePositionHashSet;
    }

    private void InitQueueAndVisited(Vector3Int start)
    {
        _queue.Clear();
        _queue.Enqueue(start);
        _visited.Clear();
        _visited.Add(start);
    }

    private bool IsPositionValid(Vector3Int position)
    {
        return _hallAreaPositionHashSet.Contains(position) 
            && !_placedPositionHashSet.Contains(position) 
            && !_visited.Contains(position);
    }
}
