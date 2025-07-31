using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private Rigidbody _rigidbody;
    private PlayerAnimationAbility _animationAbility;
    private Vector3 _lastForwardVector = Vector3.forward;
    private float _cosAngleThreshold;

    protected override void Awake()
    {
        base.Awake();

        _rigidbody = _owner.GetComponent<Rigidbody>();

        _cosAngleThreshold = Mathf.Cos(_owner.Stat.MoveAngleLimit);
    }

    private void Start()
    {
        _animationAbility = _owner.GetAbility<PlayerAnimationAbility>();
    }

    private void FixedUpdate()
    {
        //if (!_photonView.IsMine)
        //{
        //    return;
        //}

        Vector2 moveInput = InputManager.Instance.MoveInput;
        float inputSize = Mathf.Min(moveInput.magnitude, 1f);

        _animationAbility.SetBool(EPlayerAnimationParameter.IsMove, inputSize != 0f);

        moveInput.Normalize();

        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);

        if (inputSize > 0f)
        {
            _owner.transform.forward = Vector3.RotateTowards(_owner.transform.forward, inputDirection, Time.deltaTime * _owner.Stat.TurnSpeed, 0f);
            _lastForwardVector = _owner.transform.forward;
        }
        else
        {
            _owner.transform.forward = _lastForwardVector;
        }

        float dot = Vector3.Dot(_owner.transform.forward, inputDirection);

        if (dot >= _cosAngleThreshold)
        {
            _rigidbody.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * inputSize * _owner.Stat.MoveSpeed;
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
}
