using UnityEngine;

public class PlayerPingAbility : PlayerAbility
{
    private void Start()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        InputManager.Instance.OnPingEvent += Ping;
    }

    private void Ping()
    {
        Debug.Log("Ping");
    }
}
