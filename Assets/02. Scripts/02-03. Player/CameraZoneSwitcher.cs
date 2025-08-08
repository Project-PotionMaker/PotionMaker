using Unity.Cinemachine;
using UnityEngine;

public class CameraZoneSwitcher : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera _outSideCamera;

    private int _priorityOnEnter = 20;
    private int _priorityOnExit = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer(other))
        {
            return;
        }

        _outSideCamera.Priority = _priorityOnEnter;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer(other))
        {
            return;
        }

        _outSideCamera.Priority = _priorityOnExit;
    }

    private bool IsLocalPlayer(Collider other)
    {
        return other.CompareTag("Player")
            && other.TryGetComponent<Player>(out Player player)
            && player.isLocalPlayer;
    }
}
