using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;
using System.Collections.Generic;
using Utilities;
using UnityEditor.Timeline.Actions;

public class PlayerInputController : MonoBehaviour
{
    public NewPlayerController playerController;
    public PlayerInput playerInput { get; private set; }
    public PlayerInputActions playerInputActions { get; private set; }

    public string gameplayMapName { get; private set; } = "Player";
    public string UIMapName { get; private set; } = "UI";
    public string saveMenuMapName { get; private set; } = "SaveMenu";
    private InputActionMap gameplayMap;
    private InputActionMap UIMap;
    private InputActionMap saveMenuMap;

    //Input actions that need constant values
    private InputAction Move;
    private InputAction LookStick;

    public Vector2 MoveVal { get; private set; }
    public Vector2 LookStickVal { get; private set; }

    //One shot actions
    public Action<CallbackContext> OnLookMousePerformed;
    public Action<CallbackContext> OnLookStickPerformed;

    public Action<CallbackContext> OnAttackPerformed;
    public bool AttackButtonHeldDown;
    public CountdownTimer attackCountdownTimer { get; private set; }
    private float attackHeldRepeatTime = 0.1f;
    public Action<CallbackContext> OnJumpPerformed;
    public Action<CallbackContext> OnAbilityPerformed;
    public bool AbilityButtonHeldDown;
    public Action<CallbackContext> OnBlockPerformed;
    public Action<CallbackContext> OnInteractPerformed;
    public Action<CallbackContext> OnDrinkPotionOnePerformed;
    public Action<CallbackContext> OnDrinkPotionTwoPerformed;
    public Action<CallbackContext> OnCycleAbilityListLeftPerformed;
    public Action<CallbackContext> OnCycleAbilityListRightPerformed;
    public Action<CallbackContext> OnCycleWeaponSetUpPerformed;
    public Action<CallbackContext> OnCycleWeaponSetDownPerformed;

    public Action<CallbackContext> OnDropItemPerformed;
    public Action<CallbackContext> OnDisplayMoreInformationPerformed;
    public Action<CallbackContext> OnEquipOffhandPerformed;
    public Action<CallbackContext> OnCancelPerformed;
    public Action<CallbackContext> OnSubmitPerformed;

    //Stored dictionary for unsubscribing from all events
    private Dictionary<InputAction, Action<CallbackContext>> subscribedInputActions = new Dictionary<InputAction, Action<CallbackContext>>();

    public int playerIndex { get; private set; }

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.playerIndex;

        gameplayMap = playerInput.actions.FindActionMap(gameplayMapName);
        UIMap = playerInput.actions.FindActionMap(UIMapName);
        saveMenuMap = playerInput.actions.FindActionMap(saveMenuMapName);

        attackCountdownTimer = new CountdownTimer(attackHeldRepeatTime);
    }

    void OnEnable()
    {
        var equipmentMenu = playerInputActions.Player.OpenEquipmentMenu;
        equipmentMenu.Enable();

        equipmentMenu = playerInputActions.UI.OpenEquipmentMenu;
        equipmentMenu.Enable();

        Move = playerInput.currentActionMap.FindAction(playerInputActions.Player.Move.id);
        Move.Enable();

        //Need to always track stick position to determine rotation when walking using gamepad
        LookStick = playerInput.currentActionMap.FindAction(playerInputActions.Player.LookStick.id);
        LookStick.Enable();

        //These are one shot events (buttons)
        SubscribeToInputAction(playerInputActions.Player.Ability.id.ToString(), OnAbility, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.Attack.id.ToString(), OnAttack, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.Jump.id.ToString(), OnJump, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.OpenEquipmentMenu.id.ToString(), OnEquipmentMenu, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.LookMouse.id.ToString(), OnLookMouse, gameplayMap);
        //SubscribeToInputAction(playerInputActions.Player.LookStick.id.ToString(), OnLookStick, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.Interact.id.ToString(), OnInteract, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.DrinkPotionOne.id.ToString(), OnDrinkPotionOne, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.DrinkPotionTwo.id.ToString(), OnDrinkPotionTwo, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.CycleAbilityListRight.id.ToString(), OnCycleAbilityListRight, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.CycleAbilityListLeft.id.ToString(), OnCycleAbilityListLeft, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.ResetCameraRotation.id.ToString(), OnResetCameraRotation, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.CycleWeaponSetUp.id.ToString(), OnCycleWeaponSetUp, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.CycleWeaponSetDown.id.ToString(), OnCycleWeaponSetDown, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.MouseDelta.id.ToString(), OnMouseDelta, gameplayMap);

        SubscribeToInputAction(playerInputActions.UI.OpenEquipmentMenu.id.ToString(), OnEquipmentMenu, UIMap);
        SubscribeToInputAction(playerInputActions.UI.DropItem.id.ToString(), OnDropItem, UIMap);
        SubscribeToInputAction(playerInputActions.UI.MoveMenuLeft.id.ToString(), OnMoveMenuLeft, UIMap);
        SubscribeToInputAction(playerInputActions.UI.MoveMenuRight.id.ToString(), OnMoveMenuRight, UIMap);
        SubscribeToInputAction(playerInputActions.UI.EquipOffhand.id.ToString(), OnEquipOffhand, UIMap);
        SubscribeToInputAction(playerInputActions.UI.Cancel.id.ToString(), OnCancel, UIMap);
        SubscribeToInputAction(playerInputActions.UI.Submit.id.ToString(), OnSubmit, UIMap);

        SubscribeToInputAction(playerInputActions.SaveMenu.Cancel.id.ToString(), OnCancelSaveMenu, saveMenuMap);

        //Specific cases//

        //Block needs to be called when started and canceled instead of performed
        InputAction action = playerInput.currentActionMap.FindAction(playerInputActions.Player.Block.id);
        action.started += OnBlock;
        action.canceled += OnBlock;

        action = playerInput.currentActionMap.FindAction(playerInputActions.Player.Attack.id);
        action.started += OnAttackHeld;
        action.canceled += OnAttackHeld;

        action = playerInput.currentActionMap.FindAction(playerInputActions.Player.Ability.id);
        action.started += OnAbility;
        action.canceled += OnAbility;

        action = playerInput.currentActionMap.FindAction(playerInputActions.Player.LookStick.id);
        action.started += OnLookStick;
        action.canceled += OnLookStick;

        action = UIMap.FindAction(playerInputActions.UI.DisplayMoreInformation.id);
        action.started += OnDisplayMoreInformation;
        action.canceled += OnDisplayMoreInformation;

        action = playerInput.currentActionMap.FindAction(playerInputActions.Player.RotateButton.id);
        action.started += OnRotateButton;
        action.canceled += OnRotateButton;

        attackCountdownTimer.OnTimerStop += ResetAttackTimer;
    }

    void Update()
    {
        MoveVal = Move.ReadValue<Vector2>();
        LookStickVal = LookStick.ReadValue<Vector2>();

        attackCountdownTimer.Tick(Time.deltaTime);

        if (FreeLookCameraManager.Instance == null) return;
        FreeLookCameraManager.Instance.stickDelta = LookStickVal;
    }

    private void SubscribeToInputAction(string id, Action<CallbackContext> function, InputActionMap map)
    {
        InputAction action = map.FindAction(id);
        action.performed += function;
        subscribedInputActions.Add(action, function);
    }

    void OnDestroy()
    {
        foreach (KeyValuePair<InputAction, Action<CallbackContext>> entry in subscribedInputActions)
        {
            entry.Key.performed -= entry.Value;
        }

        subscribedInputActions.Clear();

        InputAction action = UIMap.FindAction(playerInputActions.UI.OpenEquipmentMenu.id);
        action.performed -= OnEquipmentMenu;

        attackCountdownTimer.OnTimerStop -= ResetAttackTimer;
    }

    public void EnablePlayerActionMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void EnableUIActionMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    #region Player Input Actions
    public void OnAbility(CallbackContext context)
    {
        if (context.started)
        {
            OnAbilityPerformed?.Invoke(context);
            AbilityButtonHeldDown = true;
        }
        else if (context.canceled)
        {
            AbilityButtonHeldDown = false;
        }
    }

    public void OnAttack(CallbackContext context)
    {
        OnAttackPerformed?.Invoke(context);
    }

    public void OnAttackHeld(CallbackContext context)
    {
        if (context.started)
        {
            AttackButtonHeldDown = true;
            attackCountdownTimer.Start();
        }
        else if (context.canceled)
        {
            AttackButtonHeldDown = false;
            attackCountdownTimer.Pause();
        }
    }

    public void OnBlock(CallbackContext context)
    {
        OnBlockPerformed?.Invoke(context);
    }

    public void OnJump(CallbackContext context)
    {
        OnJumpPerformed?.Invoke(context);
    }

    public void OnLookMouse(CallbackContext context)
    {
        OnLookMousePerformed?.Invoke(context);
    }

    public void OnLookStick(CallbackContext context)
    {
        OnLookStickPerformed?.Invoke(context);
    }

    public void OnMove(CallbackContext context)
    {

    }

    public void OnSwapWeapon(CallbackContext context)
    {

    }

    public void OnInteract(CallbackContext context)
    {
        OnInteractPerformed?.Invoke(context);
    }

    public void OnDrinkPotionOne(CallbackContext context)
    {
        OnDrinkPotionOnePerformed?.Invoke(context);
    }

    public void OnDrinkPotionTwo(CallbackContext context)
    {
        OnDrinkPotionTwoPerformed?.Invoke(context);
    }

    public void OnResetCameraRotation(CallbackContext context)
    {
        FreeLookCameraManager.Instance.OrientTowardsLookDirection(playerController.transform);
    }

    public void OnCycleAbilityListRight(CallbackContext context)
    {
        playerController.PlayerContext.UserInterfaceController.AbilityScrollController.CycleRight();
    }

    public void OnCycleAbilityListLeft(CallbackContext context)
    {
        playerController.PlayerContext.UserInterfaceController.AbilityScrollController.CycleLeft();
    }

    public void OnCycleWeaponSetUp(CallbackContext context)
    {
        playerController.WeaponController.CycleActiveWeaponSetUp();
    }

    public void OnCycleWeaponSetDown(CallbackContext context)
    {
        playerController.WeaponController.CycleActiveWeaponSetDown();
    }

    public void OnMouseDelta(CallbackContext context)
    {
        if (FreeLookCameraManager.Instance == null) return;
        FreeLookCameraManager.Instance.OnMouseDelta(context);
    }

    public void OnRotateButton(CallbackContext context)
    {
        Debug.Log($"On Rotate Button");
        if (FreeLookCameraManager.Instance == null) return;
        FreeLookCameraManager.Instance.OnRotateButton(context);
    }
    #endregion

    #region UI Input Actions
    public void OnSubmit(CallbackContext context)
    {
        OnSubmitPerformed?.Invoke(context);
    }
    public void OnCancel(CallbackContext context)
    {
        OnCancelPerformed?.Invoke(context);
    }

    public void OnDropItem(CallbackContext context)
    {
        OnDropItemPerformed?.Invoke(context);
    }

    public void OnMoveMenuLeft(CallbackContext context)
    {
        playerController.PlayerContext.UserInterfaceController.inventoryController.GoToPreviousMenu();
    }

    public void OnMoveMenuRight(CallbackContext context)
    {
        playerController.PlayerContext.UserInterfaceController.inventoryController.GoToNextMenu();
    }

    public void OnDisplayMoreInformation(CallbackContext context)
    {
        OnDisplayMoreInformationPerformed?.Invoke(context);
    }

    public void OnEquipOffhand(CallbackContext context)
    {
        OnEquipOffhandPerformed?.Invoke(context);
    }
    #endregion

    #region Save Menu Input Actions

    public void OnCancelSaveMenu(CallbackContext context)
    {
        SaveMenuManager saveMenuManager = SaveMenuManager.Instance;
        if (saveMenuManager == null) return;

        saveMenuManager.CloseSaveMenu();
    }

    #endregion

    #region Shared Input Actions
    public void OnEquipmentMenu(CallbackContext context)
    {
        playerController.PlayerContext.UserInterfaceController.TryToggleInventory();
    }
    #endregion

    private void ResetAttackTimer()
    {
        if (AttackButtonHeldDown)
        {
            attackCountdownTimer.Start();
        }
    }
}
