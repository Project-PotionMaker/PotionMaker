using UnityEngine;

public class PlayerPingAbility : PlayerAbility
{
    private void Start()
    {
        InputManager.Instance.OnPingEvent += Ping;
    }

    private void Ping()
    {
        Debug.Log("Ping");
    }
}
