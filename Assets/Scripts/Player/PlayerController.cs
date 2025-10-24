using UnityEngine;

[RequireComponent(typeof(PlayerInputController), typeof(Rigidbody), typeof(HealthController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;

    PlayerInputController _playerInputController;
    Rigidbody _rigidbody;
    private HealthController HealthController;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Hitbox _basicAttackHitbox;
    private WeaponController weaponController;
    private AbilityController abilityController;
    private Vector3 mousePosOnGround;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _rigidbody = GetComponent<Rigidbody>();
        HealthController = GetComponent<HealthController>();
        weaponController = GetComponent<WeaponController>();
        abilityController = GetComponent<AbilityController>();
    }

    private void OnEnable()
    {
        _playerInputController.OnMovePerformed += Move;
        _playerInputController.OnJumpPerformed += Jump;
        _playerInputController.OnBasicAttackPerformed += BasicAttack;
        _playerInputController.OnMouseMoved += RotateTowardsMouse;
        _playerInputController.OnAbility1Performed += Ability1;

        HealthController.OnDie += Die;
    }

    private void OnDisable()
    {
        _playerInputController.OnMovePerformed -= Move;
        _playerInputController.OnJumpPerformed -= Jump;
        _playerInputController.OnBasicAttackPerformed -= BasicAttack;
        _playerInputController.OnMouseMoved -= RotateTowardsMouse;
        _playerInputController.OnAbility1Performed -= Ability1;

        HealthController.OnDie -= Die;
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

    private void Die()
    {

    }

    private void BasicAttack()
    {
        weaponController.CallAttack(transform, transform.forward);
        //_basicAttackHitbox.ActivateHitbox(0.2f);
    }

    private void RotateTowardsMouse(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 updatedHitPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            mousePosOnGround = updatedHitPoint;
            Vector3 dirToPoint = (updatedHitPoint - transform.position).normalized;
            transform.LookAt(transform.position + dirToPoint);
        }
    }

    private void Ability1()
    {
        abilityController.CallAbility(mousePosOnGround);
    }

    #region Debug
    [ContextMenu("Take Damage")]
    public void TakeDamageTest()
    {
        HealthController.TakeDamage(1);
    }
    #endregion


}
