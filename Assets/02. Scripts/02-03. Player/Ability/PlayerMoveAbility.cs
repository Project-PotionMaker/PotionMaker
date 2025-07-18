using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private Rigidbody _rigidbody;
    private Vector3 _lastForwardVector = Vector3.forward;

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
        float inputSize = Mathf.Min(moveInput.magnitude, 1f);

        moveInput.Normalize();

        if (inputSize > 0f)
        {
            Vector3 targetForward = new Vector3(moveInput.x, 0, moveInput.y);
            _owner.transform.forward = Vector3.Lerp(_owner.transform.forward, targetForward, Time.deltaTime * 10f);
            _lastForwardVector = _owner.transform.forward;
        }
        else
        {
            _owner.transform.forward = _lastForwardVector;
        }

        _rigidbody.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * inputSize * _owner.Stat.MoveSpeed;
    }
}
