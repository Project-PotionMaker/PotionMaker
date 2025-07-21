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
        Vector3 targetPosition = _owner.GetFrontPosition();
        GameObject structure = GridManager.Instance.GetObjectOnGrid(targetPosition);
        if (ReferenceEquals(structure, null))
        {
            return;
        }

        IGridItemHandler itemHandler = structure.GetComponent<IGridItemHandler>();
        if (ReferenceEquals(itemHandler, null) == false)
        {
            itemHandler.TryInteract();
        }
    }

    private void EndInteract()
    {
        Debug.Log("EndInteract");
    }
}
