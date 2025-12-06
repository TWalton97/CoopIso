using UnityEngine;
using Utilities;
using UnityEngine.InputSystem;
using System;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;
using System.Collections;

public class NewPlayerController : Entity
{
    public FriendlySkeletonWarrior FriendlySkeletonWarrior;
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
    public bool MovementLocked = false;
    public bool blockButtonPressed;
    public bool attackButtonPressed;
    public bool abilityButtonPressed;
    public string KEYBOARD_SCHEME { get; private set; } = "Keyboard&Mouse";
    public string GAMEPAD_SCHEME { get; private set; } = "Gamepad";

    public AnimationClip animationClip;

    public List<Renderer> playerIndicator;

    public Vector3 lookPoint;
    public Vector3 rotatedInputDirection;

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
        HealthController.Init(EntityData.MaximumHealth);
        _maximumMovementSpeed = EntityData.MovementSpeed;
        PlayerStatsBlackboard.CriticalChance = EntityData.CriticalChance;
        PlayerStatsBlackboard.CriticalDamage = EntityData.CriticalDamage;
    }

    void Start()
    {
        SetupMovementStateMachine();
        SetupAttackStateMachine();
        SubscribeToInputEvents();
        // StartCoroutine(WaitForSetup());

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

        Animator.SetFloat("VelocityX", localVelocity.x / EntityData.MovementSpeed);
        Animator.SetFloat("VelocityZ", localVelocity.z / EntityData.MovementSpeed);

        Animator.SetFloat("SpeedMultiplier", Mathf.Clamp(_movementSpeed / _maximumMovementSpeed, 0.5f, 1));
    }
    #endregion

    #region Movement
    private void Move()
    {
        _moveInput = PlayerInputController.MoveVal;

        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;

        camForward.y = 0;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        rotatedInputDirection = camForward * _moveInput.y + camRight * _moveInput.x;

        Vector3 newVel = rotatedInputDirection * 0;

        if (!MovementLocked)
        {
            newVel = rotatedInputDirection * _maximumMovementSpeed;
        }

        newVel.y = Rigidbody.velocity.y;


        Rigidbody.velocity = newVel;
        _movementSpeed = newVel.magnitude;

        if (attackStateMachine.current.State == attackState || (attackStateMachine.current.State == castState && !AbilityBeingUsed.CanRotateDuringCast))
            return;

        //Blocking and aiming rotation logic
        if (attackStateMachine.current.State == blockState)
        {
            //Aim assist handling
            if (BowAimLineController.HitTarget != null)
            {
                Vector3 targetPos = new Vector3(BowAimLineController.HitTarget.transform.position.x, transform.position.y, BowAimLineController.HitTarget.transform.position.z);
                Vector3 newRotation = (targetPos - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(newRotation);
                return;
            }
            //Rotate towards mouse
            else if (PlayerContext.PlayerInput.currentControlScheme == KEYBOARD_SCHEME)
            {
                RotateToFaceLookPoint();
                return;
            }
        }

        //Rotating towards mouse while casting logic
        if (PlayerContext.PlayerInput.currentControlScheme == KEYBOARD_SCHEME && attackStateMachine.current.State == castState && AbilityBeingUsed.CanRotateDuringCast)
        {
            RotateToFaceLookPoint();
            return;
        }

        if (_moveInput.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(rotatedInputDirection);
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
        PlayerStatsBlackboard.UpdateMovementSpeed();
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

            lookPoint = dirToPoint;
        }
    }

    private void RotateTowardsStick(CallbackContext context)
    {
        lookPoint = context.ReadValue<Vector2>();
        //RotateToFaceDir(dir);
    }

    public void RotateToFaceLookPoint()
    {
        if (PlayerContext.PlayerInput.currentControlScheme == KEYBOARD_SCHEME && lookPoint.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(lookPoint);
    }

    #endregion

    #region Combat

    private void Ability(CallbackContext context)
    {
        TryCastAbility();
    }

    private void Attack(CallbackContext context)
    {
        attackButtonPressed = true;
    }

    private void Block(CallbackContext context)
    {
        if (context.started)
        {
            blockButtonPressed = true;
            Animator.SetBool("BlockButtonHeld", true);
        }

        if (context.canceled)
        {
            blockButtonPressed = false;
            Animator.SetBool("BlockButtonHeld", false);
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
        if (PotionController.drinkingPotion) return;

        ConsumableButton button = PlayerContext.UserInterfaceController.inventoryController.ConsumablesInventory.TryFindLargestPotionOfType(Resources.ResourceType.Health);
        if (button == null) return;

        PotionController.UsePotion(button.inventoryItemView.ItemSO as PotionSO);
        button.UpdateQuantity(-1);
    }

    private void DrinkPotionTwo(CallbackContext context)
    {
        ConsumableButton button = PlayerContext.UserInterfaceController.inventoryController.ConsumablesInventory.TryFindLargestPotionOfType(Resources.ResourceType.Mana);
        if (button == null) return;

        PotionController.UsePotion(button.inventoryItemView.ItemSO as PotionSO);
        button.UpdateQuantity(-1);
    }

    #endregion

    [ContextMenu("Spawn Friendly Skeleton")]
    public void SpawnFriendlySkeleton()
    {
        FriendlySkeletonWarrior archer = Instantiate(FriendlySkeletonWarrior, transform.position + transform.forward, Quaternion.identity);
        archer.Init(this);
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
