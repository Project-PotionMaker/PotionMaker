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
        if (!other.CompareTag("Player"))
        {
            return false;
        }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            return false;
        }

        if (!player.isLocalPlayer)
        {
            return false;
        }

        return true;
    }
}
