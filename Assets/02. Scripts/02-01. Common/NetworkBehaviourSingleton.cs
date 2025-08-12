using Mirror;
using System;
using UnityEngine;

public class NetworkBehaviourSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    [SerializeField]
    private bool _dontDestroy;

    private static T _instance;

    public static event Action OnInitialized;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
            }
            return _instance;
        }
    }

    public override void OnStartServer()
    {
        SetupInstance();
        if (!netIdentity.isServer)
        {
            NetworkServer.Spawn(gameObject);
        }
    }

    public override void OnStartClient()
    {
        SetupInstance();
    }

    private void SetupInstance()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"중복된 {typeof(T).Name} 인스턴스 발견. 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (_dontDestroy)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
