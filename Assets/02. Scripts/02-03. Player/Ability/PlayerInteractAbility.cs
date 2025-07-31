using UnityEngine;

public class PlayerInteractAbility : PlayerAbility
{
    private bool _isInteract = false;
    private PlayerAnimationAbility _animationAbility;


    private void Start()
    {
        if (!_owner.isLocalPlayer)
        {
            return;
        }

        InputManager.Instance.OnInteractChanged += ChangeInteractState;
        _animationAbility = _owner.GetAbility<PlayerAnimationAbility>();
    }

    private void Update()
    {
        if (_isInteract)
        {
            ProcessInteract();
        }
    }

    private void ChangeInteractState(bool isInteract)
    {
        _isInteract = isInteract;
        _animationAbility.SetBool(EPlayerAnimationParameter.IsInteract, isInteract);
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
        IGridItemHandler itemHandler = structure?.GetComponent<IGridItemHandler>();
        if (ReferenceEquals(itemHandler, null) == false)
        {
            itemHandler.TryInteract();
        }
    }

    private void ProcessInteract()
    {

    }

    private void EndInteract()
    {
        Debug.Log("EndInteract");
    }
}
