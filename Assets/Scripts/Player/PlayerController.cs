using UnityEngine;

[RequireComponent(typeof(PlayerInputController), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;

    PlayerInputController _playerInputController;
    Rigidbody _rigidbody;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _playerInputController.OnMovePerformed += Move;
        _playerInputController.OnJumpPerformed += Jump;
    }

    private void OnDisable()
    {
        _playerInputController.OnMovePerformed -= Move;
        _playerInputController.OnJumpPerformed -= Jump;
    }

    public void Move(Vector2 moveDir)
    {
        Vector3 inputDirection = new Vector3(moveDir.x, 0, moveDir.y);

        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        Vector3 newVel = rotatedInputDirection * _movementSpeed;
        newVel.y = _rigidbody.velocity.y;

        _rigidbody.velocity = newVel;
    }

    public void Jump()
    {
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }


}
