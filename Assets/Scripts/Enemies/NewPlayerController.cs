using UnityEngine;
using Utilities;
using UnityEngine.InputSystem;
using System;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;
using System.Collections;

public class NewPlayerController : Entity
{
    public PlayerContext PlayerContext;
    //Object references
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public NewWeaponController WeaponController;
    public ArmorController ArmorController;
    public ExperienceController ExperienceController;
    public PlayerInputController PlayerInputController { get; private set; }
    public GroundCheck GroundCheck { get; private set; }
    public Interactor Interactor { get; private set; }
    public AnimationStatusTracker AnimationStatusTracker { get; private set; }
    public PotionController PotionController { get; private set; }
    public PlayerUserInterfaceController PlayerUserInterfaceController { get; private set; }
    public PlayerStatsBlackboard PlayerStatsBlackboard;
    public FeatsController FeatsController;
    public PlayerAnimationController PlayerAnimationController;
    public AbilityController AbilityController;
    public ResourceController ResourceController;
    public StatusController StatusController;
    public BowAimLineController BowAimLineController { get; private set; }

    public float _movementSpeed;
    public float _maximumMovementSpeed;
    public float _jumpForce;

    public StateMachine attackStateMachine;
    public PlayerIdleState idleState;
    PlayerAttackState attackState;
    public PlayerBlockState blockState;
    public PlayerCastState castState;

    public StateMachine movementStateMachine;


    [SerializeField] private LayerMask GroundLayer = NavMeshUtils.GROUND_LAYER;
    private Vector2 _moveInput;
    public bool blockButtonPressed;
    public bool attackButtonPressed;
    public bool abilityButtonPressed;
    public string KEYBOARD_SCHEME { get; private set; } = "Keyboard&Mouse";
    public string GAMEPAD_SCHEME { get; private set; } = "Gamepad";

    public AnimationClip animationClip;

    public List<Renderer> playerIndicator;

    public Vector3 lookPoint;

    public AbilitySO AbilityBeingUsed;

    private Camera mainCam;

    #region MonoBehaviour

    public void Init()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        PlayerInputController = GetComponent<PlayerInputController>();
        GroundCheck = GetComponent<GroundCheck>();
        Interactor = GetComponentInChildren<Interactor>();
        AnimationStatusTracker = GetComponentInChildren<AnimationStatusTracker>();
        PotionController = GetComponent<PotionController>();
        BowAimLineController = GetComponentInChildren<BowAimLineController>();
        mainCam = Camera.main;
    }

    public override void ApplyStats()
    {
        base.ApplyStats();
        _movementSpeed = EntityData.MovementSpeed;
        PlayerStatsBlackboard.CriticalChance = EntityData.CriticalChance;
        PlayerStatsBlackboard.CriticalDamage = EntityData.CriticalDamage;
    }

    void Start()
    {
        SetupMovementStateMachine();
        SetupAttackStateMachine();
        SubscribeToInputEvents();
        // StartCoroutine(WaitForSetup());

        _maximumMovementSpeed = _movementSpeed;
        PlayerInputController.attackCountdownTimer.OnTimerStop += () => attackButtonPressed = true;

        if (PlayerInputController.playerIndex == 1)
        {
            foreach (Renderer rend in playerIndicator)
            {
                rend.material.color = Color.red;
            }
        }
        else
        {
            foreach (Renderer rend in playerIndicator)
            {
                rend.material.color = Color.blue;
            }
        }
    }

    void OnDisable()
    {
        UnsubscribeFromInputEvents();
        PlayerInputController.attackCountdownTimer.OnTimerStop -= () => attackButtonPressed = true;
    }

    private void SubscribeToInputEvents()
    {
        InventoryManager.OnMenuOpened += () => PlayerInputController.EnableUIActionMap();
        InventoryManager.OnMenuClosed += () => PlayerInputController.EnablePlayerActionMap();
        InventoryManager.OnMenuClosed += () => attackStateMachine.ChangeState(idleState);

        PlayerInputController.OnLookMousePerformed += RotateTowardsMouse;
        PlayerInputController.OnLookStickPerformed += RotateTowardsStick;

        PlayerInputController.OnAbilityPerformed += Ability;
        PlayerInputController.OnAttackPerformed += Attack;
        PlayerInputController.OnJumpPerformed += Jump;
        PlayerInputController.OnBlockPerformed += Block;
        PlayerInputController.OnInteractPerformed += Interact;
        PlayerInputController.OnDrinkPotionOnePerformed += DrinkPotionOne;
        PlayerInputController.OnDrinkPotionTwoPerformed += DrinkPotionTwo;
    }

    private void UnsubscribeFromInputEvents()
    {
        InventoryManager.OnMenuOpened -= () => PlayerInputController.EnableUIActionMap();
        InventoryManager.OnMenuClosed -= () => PlayerInputController.EnablePlayerActionMap();
        InventoryManager.OnMenuClosed -= () => attackStateMachine.ChangeState(idleState);

        //PlayerInputController.OnLookMousePerformed -= RotateTowardsMouse;
        //PlayerInputController.OnLookStickPerformed -= RotateTowardsStick;

        PlayerInputController.OnAbilityPerformed -= Ability;
        PlayerInputController.OnAttackPerformed -= Attack;
        PlayerInputController.OnJumpPerformed -= Jump;
        PlayerInputController.OnBlockPerformed -= Block;
        PlayerInputController.OnInteractPerformed -= Interact;
        PlayerInputController.OnDrinkPotionOnePerformed -= DrinkPotionOne;
        PlayerInputController.OnDrinkPotionTwoPerformed -= DrinkPotionTwo;

    }

    void Update()
    {
        StateMachineUpdate();
        UpdateAnimatorParameters();
    }
    void FixedUpdate()
    {
        StateMachineFixedUpdate();
        Move();
    }
    #endregion

    #region State Machines

    private void StateMachineUpdate()
    {
        attackStateMachine.Update();
        movementStateMachine.Update();
    }

    private void StateMachineFixedUpdate()
    {
        movementStateMachine.FixedUpdate();
        attackStateMachine.FixedUpdate();
    }

    //Transitions
    void At(IState from, IState to, StateMachine machine, IPredicate condition) => machine.AddTransition(from, to, condition);
    void Any(IState to, StateMachine machine, IPredicate condition) => machine.AddAnyTransition(to, condition);

    private void SetupMovementStateMachine()
    {
        movementStateMachine = new StateMachine();

        var groundedState = new PlayerGroundedState(this, Animator);
        var airborneState = new PlayerAirborneState(this, Animator);

        At(groundedState, airborneState, movementStateMachine, new FuncPredicate(() => !GroundCheck.Grounded));
        At(airborneState, groundedState, movementStateMachine, new FuncPredicate(() => GroundCheck.Grounded));

        movementStateMachine.SetState(groundedState);
    }

    private void SetupAttackStateMachine()
    {
        attackStateMachine = new StateMachine();

        idleState = new PlayerIdleState(this, Animator);
        attackState = new PlayerAttackState(this, Animator);
        blockState = new PlayerBlockState(this, Animator);
        castState = new PlayerCastState(this, Animator);

        At(idleState, blockState, attackStateMachine, new FuncPredicate(() => blockButtonPressed));
        At(blockState, idleState, attackStateMachine, new FuncPredicate(() => !blockButtonPressed));

        At(idleState, attackState, attackStateMachine, new FuncPredicate(() => attackButtonPressed));
        At(blockState, attackState, attackStateMachine, new FuncPredicate(() => attackButtonPressed));

        At(idleState, castState, attackStateMachine, new FuncPredicate(() => abilityButtonPressed && PlayerUserInterfaceController.AbilityScrollController.AbilityReadyToBeUsed()));
        At(blockState, castState, attackStateMachine, new FuncPredicate(() => abilityButtonPressed && PlayerUserInterfaceController.AbilityScrollController.AbilityReadyToBeUsed()));

        //Any(attackState, attackStateMachine, new FuncPredicate(() => attackButtonPressed));

        attackStateMachine.SetState(idleState);
    }

    #endregion

    #region Animator

    private void UpdateAnimatorParameters()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity);

        Animator.SetFloat("VelocityX", localVelocity.x / _maximumMovementSpeed);
        Animator.SetFloat("VelocityZ", localVelocity.z / _maximumMovementSpeed);

        Animator.SetFloat("SpeedMultiplier", Mathf.Clamp(_movementSpeed / _maximumMovementSpeed, 0.5f, 1));
    }
    #endregion

    #region Movement
    private void Move()
    {
        _moveInput = PlayerInputController.MoveVal;
        Vector3 inputDirection = new Vector3(_moveInput.x, 0, _moveInput.y);

        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;

        camForward.y = 0;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 rotatedInputDirection = camForward * _moveInput.y + camRight * _moveInput.x;

        //Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        //Vector3 rotatedInputDirection = rotation * inputDirection;

        Vector3 newVel = rotatedInputDirection * _movementSpeed;
        newVel.y = Rigidbody.velocity.y;

        Rigidbody.velocity = newVel;

        if (_moveInput.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(rotatedInputDirection);



        // if (attackStateMachine.current.State == attackState || attackStateMachine.current.State == blockState || (attackStateMachine.current.State == castState && AbilityBeingUsed.CanRotateDuringCast))
        // {
        //     if (PlayerInputController.playerInput.currentControlScheme == GAMEPAD_SCHEME)
        //     {
        //         if (PlayerInputController.LookStickVal != Vector2.zero)
        //         {
        //             RotateToFaceDir(lookPoint);
        //         }
        //         else if (PlayerInputController.MoveVal != Vector2.zero)
        //         {
        //             RotateToFaceDir(rotatedInputDirection);
        //         }
        //     }
        //     else if (PlayerInputController.playerInput.currentControlScheme == KEYBOARD_SCHEME)
        //     {
        //         transform.LookAt(transform.position + lookPoint);
        //     }
        // }
        // else if (!(attackStateMachine.current.State == castState && !AbilityBeingUsed.CanRotateDuringCast))
        // {
        //     RotateToFaceDir(rotatedInputDirection);
        // }
    }

    private void Jump(CallbackContext context)
    {
        if (!GroundCheck.Grounded) return;
        Animator.CrossFade(Animator.StringToHash("Jump"), 0.2f, (int)PlayerAnimatorLayers.FullBody);
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    public void IncreaseMovementSpeed(float movementSpeedIncreaseAmount)
    {
        _movementSpeed += movementSpeedIncreaseAmount;
        _maximumMovementSpeed += movementSpeedIncreaseAmount;
    }

    #endregion

    #region Looking
    private void RotateTowardsMouse(CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 updatedHitPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            Vector3 dirToPoint = (updatedHitPoint - transform.position).normalized;

            //transform.LookAt(transform.position + dirToPoint);
            //lookPoint = transform.position + dirToPoint;
            lookPoint = dirToPoint;
        }
    }

    private void RotateTowardsStick(CallbackContext context)
    {
        lookPoint = context.ReadValue<Vector2>();
        //RotateToFaceDir(dir);
    }

    private void RotateToFaceDir(Vector2 dir)
    {
        Vector3 inputDirection = new Vector3(dir.x, 0, dir.y);
        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        transform.LookAt(transform.position + rotatedInputDirection);
    }

    #endregion

    #region Combat

    private void Ability(CallbackContext context)
    {
        TryCastAbility();
    }

    private void Attack(CallbackContext context)
    {
        //transform.LookAt(lookPoint);

        attackButtonPressed = true;
    }

    private void Block(CallbackContext context)
    {
        if (context.started)
        {
            blockButtonPressed = true;
        }

        if (context.canceled)
        {
            blockButtonPressed = false;
        }
    }

    private void TryCastAbility()
    {
        if (attackStateMachine.current.State == idleState || attackStateMachine.current.State == blockState)
        {
            if (PlayerContext.UserInterfaceController.AbilityScrollController.AbilityReadyToBeUsed())
            {
                attackStateMachine.ChangeState(castState);
                AbilityBeingUsed = PlayerContext.UserInterfaceController.AbilityScrollController.ActiveAbility.AbilitySO;
            }
        }
    }

    #endregion

    #region Interactions

    private void Interact(CallbackContext context)
    {
        Interactor.Interact();
    }

    private void DrinkPotionOne(CallbackContext context)
    {
        //We need to look in our inventory for what potion is in this slot
        //Get the data from that potion
        //Send it to the potion controller

        List<EquippedSlot> potionSlots = PlayerContext.InventoryController.FindEquippedSlotOfType(Slot.Potion);
        if (!potionSlots[0].slotInUse) return;

        PotionSO itemData = potionSlots[0].itemData.data as PotionSO;
        PotionController.UsePotion(itemData);
    }

    private void DrinkPotionTwo(CallbackContext context)
    {
        List<EquippedSlot> potionSlots = PlayerContext.InventoryController.FindEquippedSlotOfType(Slot.Potion);
        if (!potionSlots[1].slotInUse) return;

        PotionSO itemData = potionSlots[1].itemData.data as PotionSO;
        PotionController.UsePotion(itemData);
    }

    #endregion

    [ContextMenu("Print Animation Clip Length")]
    public void PrintAnimationClipLength()
    {
        Debug.Log(animationClip.length);
    }

    // private IEnumerator WaitForSetup()
    // {
    //     while (InventoryController == null)
    //     {
    //         InventoryController = InventoryManager.Instance.GetInventoryControllerByIndex(PlayerInputController.playerIndex);
    //         PlayerUserInterfaceController = InventoryManager.Instance.GetPlayerUserInterfaceControllerByIndex(PlayerInputController.playerIndex);
    //         yield return new WaitForSeconds(0.1f);
    //     }
    //     yield return null;
    // }

}
