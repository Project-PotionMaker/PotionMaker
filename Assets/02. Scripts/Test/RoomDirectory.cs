using System.Collections.Generic;
using UnityEngine;

public class RoomDirectory : MonoBehaviour
{
    public static RoomDirectory Instance { get; private set; }
    private readonly Dictionary<string, string> _map = new();

    private void Awake() => Instance = this;

    public void Register(string code, string ip)
    {
        _map[code] = ip;
    }

    public string GetAddress(string code)
    {
        return _map.TryGetValue(code, out var addr) ? addr : null;
    }
}