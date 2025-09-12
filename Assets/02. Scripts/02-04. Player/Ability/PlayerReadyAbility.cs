using UnityEngine;

public class PlayerReadyAbility : PlayerAbility
{
    private bool _isReady = false;

    private void Start()
    {
        if (!_owner.isLocalPlayer)
        {
            return;
        }

        InputManager.Instance.OnReadyEvent += Ready;
    }

    private void Ready()
    {
        _isReady = !_isReady;
        Debug.Log($"Ready : {_isReady}");
    }

    private void OnDestroy()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnReadyEvent -= Ready;
        }
    }
}
