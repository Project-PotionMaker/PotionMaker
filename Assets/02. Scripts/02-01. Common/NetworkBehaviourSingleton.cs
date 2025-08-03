using Mirror;
using UnityEngine;
public class NetworkBehaviourSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    [SerializeField]
    private bool _dontDestroy;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 매우 위험한 로직. Awake가 이미 호출되어 있어야 함.
                // 만약 이 시점에 _instance가 null이라면, 네트워크 관련 초기화가 되지 않았을 가능성이 높습니다.
                // 따라서 이 프로퍼티는 'Awake에서 초기화된다'는 가정 하에 사용해야 합니다.
                UnityEngine.Debug.LogError($"{typeof(T)}가 아직 초기화되지 않았습니다. Awake() 또는 OnStartClient()가 호출될 때까지 기다리세요.");
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
                Debug.Log("CurrencyManager 인스턴스 초기화 완료 (Awake).");
            }
        }
        else
        {
            Debug.LogWarning("중복된 CurrencyManager 인스턴스 발견. 파괴합니다.");
            Destroy(gameObject);
        }
    }
}