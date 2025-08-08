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
    private Vector3Int _entrancePosition = new(-4, 0, -4); // Init 이후 불변
    private Vector3Int _exitPosition = new(4, 0, -4); // Init 이후 불변

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
        HashSet<Vector3Int> pickupTablePositionHashSet)
    {
        _hallAreaPositionHashSet = gridPositionHashSet;
        _placedPositionHashSet = placedPositionHashSet;
        _cashierPosition = cashierPosition;
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
        // 입구 -> 캐셔
        if (!BFS(_entrancePosition, _cashierPosition))
        {
            return false;
        }

        // 캐셔 -> 출구
        if (!BFS(_cashierPosition, _exitPosition))
        {
            return false;
        }

        // 캐셔 -> 모든 의자
        foreach (var chairPos in _chairPositionHashSet)
        {
            if (!BFS(_cashierPosition, chairPos))
            {
                return false;
            }
        }

        // 모든 의자 -> 출구
        foreach (var chairPos in _chairPositionHashSet)
        {
            if (!BFS(chairPos, _exitPosition))
            {
                return false;
            }
        }

        // 모든 의자 -> 모든 픽업 테이블
        foreach (var chairPos in _chairPositionHashSet)
        {
            foreach (var pickupTablePos in _pickupTablePositionHashSet)
            {
                if (!BFS(chairPos, pickupTablePos))
                {
                    return false;
                }
            }
        }

        // 모든 픽업 테이블 -> 출구
        foreach (var pickupTablePos in _pickupTablePositionHashSet)
        {
            if (!BFS(pickupTablePos, _exitPosition))
            {
                return false;
            }
        }

        return true;
    }

    private bool BFS(Vector3Int start, Vector3Int destination)
    {
        _queue.Clear();
        _queue.Enqueue(start);
        _visited.Clear();
        _visited.Add(start);

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

    private bool IsPositionValid(Vector3Int nextPosition)
    {
        return _hallAreaPositionHashSet.Contains(nextPosition) 
            && !_placedPositionHashSet.Contains(nextPosition) 
            && !_visited.Contains(nextPosition);
    }
}
