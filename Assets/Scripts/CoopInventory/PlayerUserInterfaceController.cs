using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUserInterfaceController : MonoBehaviour
{
    public PlayerContext playerContext;

    public GameObject PlayerInventoryPanel;
    public GameObject PlayerFeatsPanel;
    public GameObject PlayerResourcePanel;

    public InventoryController inventoryController;
    public PlayerFeatsPanelController featsPanelController;
    public ResourcePanelController resourcePanelController;
    public AbilityScrollController AbilityScrollController;

    private InventoryManager inventoryManager;
    public int PlayerIndex { get; private set; }
    private PlayerInput playerInput;
    public bool IsMenuOpened { get; private set; }

    private EventSystem eventSystem;

    void Awake()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Start()
    {
        SetupInventoryMenu(playerInput);
        resourcePanelController.Init(playerContext.PlayerController);
    }

    public void OpenInventoryPanel()
    {
        eventSystem.SetSelectedGameObject(inventoryController.equipmentSlot[0].gameObject);
        PlayerInventoryPanel.SetActive(true);
        PlayerFeatsPanel.SetActive(false);
        IsMenuOpened = true;
    }

    public void OpenFeatsPanel()
    {
        eventSystem.SetSelectedGameObject(featsPanelController.featButtons[0].gameObject);
        PlayerInventoryPanel.SetActive(false);
        PlayerFeatsPanel.SetActive(true);
        IsMenuOpened = true;
    }

    public void CloseAllMenus()
    {
        PlayerInventoryPanel.SetActive(false);
        PlayerFeatsPanel.SetActive(false);
        IsMenuOpened = false;
    }

    public void GoToNextMenu()
    {
        if (PlayerInventoryPanel.activeSelf)
        {
            OpenFeatsPanel();
        }
        else if (PlayerFeatsPanel.activeSelf)
        {
            inventoryController.UpdatePlayerStats();
            OpenInventoryPanel();
        }
    }

    public void Init(PlayerInput _playerInput, PlayerContext context)
    {
        PlayerIndex = _playerInput.playerIndex;
        playerInput = _playerInput;
        playerContext = context;
        inventoryManager = playerContext.InventoryManager;
        inventoryController.playerUserInterfaceController = this;

        SetupFeatsMenu();
    }

    public void SetupInventoryMenu(PlayerInput playerInput)
    {
        inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].EquipmentMenuObject = PlayerInventoryPanel;
        inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].controller = inventoryController;
        inventoryManager.EquipmentMenuObjects[playerInput.playerIndex].playerUserInterfaceController = this;
        inventoryController.playerIndex = playerInput.playerIndex;
    }

    public void SetupFeatsMenu()
    {
        featsPanelController.CreateFeatButtons(playerContext.PlayerController);
    }

    public void DisplayPlayerResourcePanel(bool value)
    {
        PlayerResourcePanel.SetActive(value);
    }

    public void AddAbility(AbilitySO ability, AbilityBehaviourBase behaviour)
    {
        AbilityScrollController.AddAbility(ability, behaviour);
    }

}
