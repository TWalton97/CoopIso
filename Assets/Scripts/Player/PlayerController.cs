using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputController), typeof(Rigidbody))]
public class PlayerController : BaseUnitController
{
    public float _movementSpeed;
    public float _jumpForce;

    PlayerInputController PlayerInputController;
    public Rigidbody Rigidbody;
    public LayerMask GroundLayer;
    private Animator Animator;

    private WeaponController _weaponController;
    private AbilityController _abilityController;
    private ResourceController _resourceController;

    private Vector3 _mousePosOnGround;

    public override void Awake()
    {
        base.Awake();

        PlayerInputController = GetComponent<PlayerInputController>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        _weaponController = GetComponent<WeaponController>();
        _abilityController = GetComponent<AbilityController>();
        _resourceController = GetComponent<ResourceController>();
    }

    public void At(StateMachine stateMachine, IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    public void Any(StateMachine stateMachine, IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    public override void OnEnable()
    {
        base.OnEnable();

        PlayerInputController.OnMovePerformed += Move;
        PlayerInputController.OnJumpPerformed += Jump;
        PlayerInputController.OnBasicAttackPerformed += BasicAttack;
        PlayerInputController.OnMouseMoved += RotateTowardsMouse;
        PlayerInputController.OnAbility1Performed += Ability1;
    }

    public override void OnDisable()
    {
        base.OnEnable();

        PlayerInputController.OnMovePerformed -= Move;
        PlayerInputController.OnJumpPerformed -= Jump;
        PlayerInputController.OnBasicAttackPerformed -= BasicAttack;
        PlayerInputController.OnMouseMoved -= RotateTowardsMouse;
        PlayerInputController.OnAbility1Performed -= Ability1;
    }

    private void Update()
    {
        //Animator.SetFloat("MovementSpeed", Rigidbody.velocity.magnitude * Mathf.Sign(Rigidbody.velocity.z) / _movementSpeed);
        Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity);
        Animator.SetFloat("VelocityX", Mathf.Clamp(localVelocity.x, -1, 1));
        Animator.SetFloat("VelocityZ", Mathf.Clamp(localVelocity.z, -1, 1));
    }

    public void Move(Vector2 moveDir)
    {
        Vector3 inputDirection = new Vector3(moveDir.x, 0, moveDir.y);

        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        Vector3 newVel = rotatedInputDirection * _movementSpeed;
        newVel.y = Rigidbody.velocity.y;

        Rigidbody.velocity = newVel;
    }

    public void Jump()
    {
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    public override void Die()
    {
        base.Die();
    }

    private void BasicAttack()
    {
        Animator.SetInteger("AttackInputs", Animator.GetInteger("AttackInputs") + 1);
        //Animator.SetTrigger("HorizontalAttack");
        _weaponController.CallAttack(transform, transform.forward);
        //_basicAttackHitbox.ActivateHitbox(0.2f);
    }

    private void RotateTowardsMouse(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 updatedHitPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            _mousePosOnGround = updatedHitPoint;
            Vector3 dirToPoint = (updatedHitPoint - transform.position).normalized;
            transform.LookAt(transform.position + dirToPoint);
        }
    }

    private void Ability1()
    {
        if (_resourceController.resource.RemoveResource(_abilityController.ability.AbilityCost))
        {
            _abilityController.CallAbility(_mousePosOnGround, transform);
        }
    }

    private IEnumerator RegenerateMana()
    {
        yield return new WaitForSeconds(1f);
        _resourceController.resource.AddResource(1f);
        StartCoroutine(RegenerateMana());
        yield return null;
    }

    public void ReduceAttackCounter()
    {
        Animator.SetInteger("AttackInputs", Mathf.Clamp(Animator.GetInteger("AttackInputs") - 1, 0, 1));
    }
}
