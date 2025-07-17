using UnityEngine;

public class PlayerInteractAbility : PlayerAbility
{
    private bool _isInteract = false;

    private void Start()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        InputManager.Instance.OnInteractChanged += ChangeInteractState;
    }

    private void Update()
    {
        if (_isInteract)
        {
            Debug.Log("Interacting");
        }
    }

    private void ChangeInteractState(bool isInteract)
    {
        _isInteract = isInteract;
        if (isInteract)
        {
            StartInteract();
        }
        else
        {
            EndInteract();
        }
    }

    private void StartInteract()
    {
        Debug.Log("StartInteract");
    }

    private void EndInteract()
    {
        Debug.Log("EndInteract");
    }
}
