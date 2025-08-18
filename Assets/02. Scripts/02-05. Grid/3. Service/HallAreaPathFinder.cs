using System;
using System.Collections.Generic;
using System.Linq;
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

    public HallAreaPathFinder()
    {
    }

    // InitGridManager가 호출되는, 레이아웃이 변경되었을 때 최초 1회 호출
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

    // 가구 배치가 바뀔 때마다 호출
    public void UpdateGridPathFinder
        (HashSet<Vector3Int> placedPositionHashSet,
        HashSet<Vector3Int> pickupTablePositionHashSet)
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
            || _placedPositionHashSet.Contains(_exitPosition))
        {
            return false;
        }

        // 입구 -> 계산대
        if (!BFS(_entrancePosition, _cashierPosition))
        {
            return false;
        }

        // 계산대 -> 출구
        if (!BFS(_cashierPosition, _exitPosition))
        {
            return false;
        }

        // 계산대 -> 모든 의자
        if (!BFSToSeveralDestinations(_cashierPosition, _chairPositionHashSet))
        {
            return false;
        }

        // 출구 -> 모든 의자
        if (!BFSToSeveralDestinations(_exitPosition, _chairPositionHashSet))
        {
            return false;
        }

        // 출구 -> 모든 픽업 테이블
        if (!BFSToSeveralDestinations(_exitPosition, _pickupTablePositionHashSet))
        {
            return false;
        }

        // 모든 의자 -> 모든 픽업 테이블
        foreach (var chairPosition in _chairPositionHashSet)
        {
            if (!BFSToSeveralDestinations(chairPosition, _pickupTablePositionHashSet))
            {
                return false;
            }
        }

        Debug.Log("경로 존재");
        return true;
    }

    private bool BFS(Vector3Int start, Vector3Int destination)
    {
        InitQueueAndVisited(start);

        while (_queue.TryDequeue(out Vector3Int currentPosition))
        {
            Vector3Int nextPosition;
            for (int i = 0; i < 4; i++)
            {
                nextPosition = currentPosition + _directions[i];
                if (nextPosition == destination)
                {
                    return true;
                }
                if (IsPositionValid(nextPosition))
                {
                    _visited.Add(nextPosition);
                    _queue.Enqueue(nextPosition);
                }
            }
        }
        return false;
    }

    private bool BFSToSeveralDestinations(Vector3Int start, HashSet<Vector3Int> destinationHashSet)
    {
        InitQueueAndVisited(start);
        HashSet<Vector3Int> reachableDestinationHashSet = new();
        while (_queue.TryDequeue(out Vector3Int currentPosition))
        {
            Vector3Int nextPosition;
            for (int i = 0; i < 4; i++)
            {
                nextPosition = currentPosition + _directions[i];
                if (destinationHashSet.Contains(nextPosition) 
                    && !reachableDestinationHashSet.Contains(nextPosition))
                {
                    reachableDestinationHashSet.Add(nextPosition);
                }
                if (IsPositionValid(nextPosition))
                {
                    _visited.Add(nextPosition);
                    _queue.Enqueue(nextPosition);
                }
            }
        }

        if (destinationHashSet.Count == reachableDestinationHashSet.Count)
        {
            return true;
        }
        return false;
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
