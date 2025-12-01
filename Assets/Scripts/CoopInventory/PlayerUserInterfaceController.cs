using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerUserInterfaceController : MonoBehaviour
{
    public PlayerContext playerContext;

    public InventoryController inventoryController;
    public GameObject PlayerControlsPanel;
    public ResourcePanelController resourcePanelController;
    public AbilityScrollController AbilityScrollController;

    public TMP_Text GoldAmountText;
    public TMP_Text WeightText;

    public bool IsMenuOpened { get; private set; }
    public EventSystem eventSystem;

    public void Init(PlayerContext context)
    {
        eventSystem = GetComponent<EventSystem>();

        playerContext = context;

        resourcePanelController.Init(playerContext.PlayerController);
        inventoryController.Init(playerContext);

        UpdateGoldAmount(0);
        UpdateWeightAmount(0, 150);

        playerContext.PlayerController.PlayerInputController.OnDropItemPerformed += CallDropItemOnButton;
        playerContext.PlayerController.PlayerInputController.OnEquipOffhandPerformed += CallEquipOffhandOnButton;
    }

    void OnDestroy()
    {
        playerContext.PlayerController.PlayerInputController.OnDropItemPerformed -= CallDropItemOnButton;
        playerContext.PlayerController.PlayerInputController.OnEquipOffhandPerformed -= CallEquipOffhandOnButton;
    }

    private void CallDropItemOnButton(CallbackContext context)
    {
        if (eventSystem.currentSelectedGameObject.TryGetComponent(out ItemButton button))
        {
            button.OnDropItem(context);
        }
    }

    private void CallEquipOffhandOnButton(CallbackContext context)
    {
        if (eventSystem.currentSelectedGameObject.TryGetComponent(out ItemButton button))
        {
            button.OnEquipOffhand(context);
        }
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

    public void UpdateGoldAmount(int current)
    {
        GoldAmountText.text = current.ToString();
    }

    public void UpdateWeightAmount(float current, float max)
    {
        WeightText.text = current.ToString("0.0") + "/" + max.ToString("0.0");
    }

}
