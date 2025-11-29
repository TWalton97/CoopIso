using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUserInterfaceController : MonoBehaviour
{
    public PlayerContext playerContext;

    public InventoryController inventoryController;
    public GameObject PlayerControlsPanel;
    public ResourcePanelController resourcePanelController;
    public AbilityScrollController AbilityScrollController;


    public bool IsMenuOpened { get; private set; }
    public EventSystem eventSystem;

    public void Init(PlayerContext context)
    {
        eventSystem = GetComponent<EventSystem>();

        playerContext = context;

        resourcePanelController.Init(playerContext.PlayerController);
        inventoryController.Init(playerContext);
    }

    public void ToggleInventory(bool toggle)
    {
        IsMenuOpened = toggle;
        inventoryController.ToggleInventory(toggle);
        TogglePlayerControlsPanel(toggle);
    }

    public void ToggleResourcePanel(bool value)
    {
        resourcePanelController.gameObject.SetActive(value);
    }

    public void TogglePlayerControlsPanel(bool value)
    {
        PlayerControlsPanel.SetActive(value);
    }

    public void AddAbility(AbilitySO ability, AbilityBehaviourBase behaviour)
    {
        AbilityScrollController.AddAbility(ability, behaviour);
    }

}
