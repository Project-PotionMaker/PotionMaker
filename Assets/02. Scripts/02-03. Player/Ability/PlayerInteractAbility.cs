using Mirror;
using UnityEngine;

public class PlayerInteractAbility : PlayerAbility
{
    private bool _isInteract = false;
    private PlayerAnimationAbility _animationAbility;
    private IGridItemHandler _currentInteractable = null;

    public NetworkIdentity HeldItemIdentity => _owner.GetAbility<PlayerPickupAbility>().HeldItemIdentity;

    private void Start()
    {
        if (!_owner.isLocalPlayer)
        {
            return;
        }

        InputManager.Instance.OnInteractChanged += ChangeInteractState;
        _animationAbility = _owner.GetAbility<PlayerAnimationAbility>();
    }

    //private void Update()
    //{
    //    if (_isInteract)
    //    {
    //        ProcessInteract();
    //    }
    //}

    private void ChangeInteractState(bool isInteract)
    {
        _isInteract = isInteract;
        if (isInteract)
        {
            StartInteract();
            if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase && HeldItemIdentity != null)
            {
                StartRefund();
                return;
            }
        }
        else
        {
            EndInteract();
            if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase && HeldItemIdentity != null)
            {
                EndRefund();
                return;
            }
        }
        _animationAbility.SetBool(EPlayerAnimationParameter.IsInteract, isInteract);

    }

    private void StartInteract()
    {
        Vector3 targetPosition = _owner.GetFrontPosition();
        GameObject structure = GridManager.Instance.GetObjectOnGrid(targetPosition);
        _currentInteractable = structure?.GetComponent<IGridItemHandler>();
        if(ReferenceEquals(_currentInteractable, null) == false)
        {
            _currentInteractable.TryInteract(_owner.connectionToClient);
        }
    }

    //private void ProcessInteract()
    //{

    //}

    private void EndInteract()
    {
        Debug.Log("EndInteract");
        if(_currentInteractable != null && _owner.isLocalPlayer)
        {
            // _currentInteractable의 TryEndInteract 호출 (아직 미구현)
        }
    }

    [Client]
    public void ReceiveInteractResult(bool success)
    {
        if (success)
        {
            Debug.Log("Interact 성공");
        }
        else
        {
            Debug.Log("Interact 실패");
        }
    }

    private void StartRefund()
    {
        if (HeldItemIdentity.TryGetComponent(out IRefundable refundTarget))
        {
            refundTarget.StartRefund();
        }
    }

    private void EndRefund()
    {
        if (HeldItemIdentity.TryGetComponent(out IRefundable refundTarget))
        {
            refundTarget.CancelRefund();
        }
        
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnInteractChanged -= ChangeInteractState;
    }
}
