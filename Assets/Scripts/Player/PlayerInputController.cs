using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;
using System.Collections.Generic;

public class PlayerInputController : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    private PlayerInputActions playerInputActions;

    private const string gameplayMapName = "Player";
    private const string UIMapName = "UI";
    private InputActionMap gameplayMap;
    private InputActionMap UIMap;

    //Input actions that need constant values
    private InputAction Move;
    private InputAction LookStick;

    public Vector2 MoveVal { get; private set; }
    public Vector2 LookStickVal { get; private set; }

    //One shot actions
    public Action<CallbackContext> OnLookMousePerformed;
    public Action<CallbackContext> OnLookStickPerformed;

    public Action<CallbackContext> OnAttackPerformed;
    public Action<CallbackContext> OnJumpPerformed;
    public Action<CallbackContext> OnAbility1Performed;
    public Action<CallbackContext> OnBlockPerformed;

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
        SubscribeToInputAction(playerInputActions.Player.Attack.id.ToString(), OnAttack, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.Jump.id.ToString(), OnJump, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.Ability1.id.ToString(), OnAbility1, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.OpenEquipmentMenu.id.ToString(), OnEquipmentMenu, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.LookMouse.id.ToString(), OnLookMouse, gameplayMap);
        SubscribeToInputAction(playerInputActions.Player.LookStick.id.ToString(), OnLookStick, gameplayMap);

        SubscribeToInputAction(playerInputActions.UI.OpenEquipmentMenu.id.ToString(), OnEquipmentMenu, UIMap);

        //Specific cases//

        //Block needs to be called when started and canceled instead of performed
        InputAction action = playerInput.currentActionMap.FindAction(playerInputActions.Player.Block.id);
        action.started += OnBlock;
        action.canceled += OnBlock;
    }

    void Update()
    {
        MoveVal = Move.ReadValue<Vector2>();
        LookStickVal = LookStick.ReadValue<Vector2>();
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
        action.performed += OnEquipmentMenu;
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
    public void OnAbility1(CallbackContext context)
    {
        OnAbility1Performed?.Invoke(context);
    }

    public void OnAttack(CallbackContext context)
    {
        OnAttackPerformed?.Invoke(context);
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
    #endregion

    #region UI Input Actions
    public void OnCancel(CallbackContext context)
    {

    }

    public void OnClick(CallbackContext context)
    {

    }

    public void OnMiddleClick(CallbackContext context)
    {

    }

    public void OnNavigate(CallbackContext context)
    {

    }

    public void OnPoint(CallbackContext context)
    {

    }

    public void OnRightClick(CallbackContext context)
    {

    }

    public void OnScrollWheel(CallbackContext context)
    {

    }

    public void OnSubmit(CallbackContext context)
    {

    }

    public void OnTrackedDeviceOrientation(CallbackContext context)
    {

    }

    public void OnTrackedDevicePosition(CallbackContext context)
    {

    }
    #endregion

    #region Shared Input Actions
    public void OnEquipmentMenu(CallbackContext context)
    {
        InventoryManager.Instance.Equipment(playerIndex);
    }
    #endregion
}
