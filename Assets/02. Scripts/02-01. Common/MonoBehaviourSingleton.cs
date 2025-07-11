using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _dontDestroy;

    [SerializeField]
    private bool _lazyInitialization;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (ReferenceEquals(_instance, null))
            {
                _instance = FindAnyObjectByType<T>();
                if (ReferenceEquals(_instance, null))
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                    if (_instance is MonoBehaviourSingleton<T> singletonInstance && singletonInstance._dontDestroy)
                    {
                        DontDestroyOnLoad(obj);
                    }
                }
            }
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if (!_lazyInitialization)
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (_dontDestroy)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
