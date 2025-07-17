using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private Rigidbody _rigidbody;

    protected override void Awake()
    {
        base.Awake();

        _rigidbody = _owner.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        Vector2 moveInput = InputManager.Instance.MoveInput;

        _rigidbody.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * _owner.MoveSpeed;
    }
}
