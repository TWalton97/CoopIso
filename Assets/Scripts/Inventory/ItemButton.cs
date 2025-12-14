using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public abstract class ItemButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public PlayerContext PlayerContext;
    public InventoryItemController InventoryItemController;
    public ItemSO ItemSO;
    public ItemData ItemData;
    public InventoryItemView inventoryItemView;
    public Color BaseColor;
    public Color EquippedColor;
    public Color CannotBeUsedColor;
    public Image[] BackgroundImages;

    public bool SlotInUse;
    public bool IsSelected;

    public TMP_Text ItemName;
    public TMP_Text ItemValue;
    public TMP_Text ItemWeight;
    public Image ItemButtonImage;
    public GameObject HighlightIcon;

    private Selectable selectable;

    public string ButtonID;

    public enum ButtonState
    {
        Default,
        Activated,
        CannotActivate
    }
    public ButtonState buttonState = ButtonState.Default;
    public InventoryMode ItemMode;

    void Awake()
    {
        selectable = GetComponent<Button>();
    }

    public abstract void InitializeItemButton(InventoryItemController inventoryItemController, PlayerContext playerContext, InventoryItemView inventoryItemView, bool isEquipped = false, InventoryMode mode = InventoryMode.Normal, bool equipToOffhand = false);
    public abstract void UpdateUI();
    public abstract void OnRightClick();
    public abstract void OnLeftClick();
    public abstract void ActivateButton();

    public void ToggleHighlight(bool toggle)
    {
        HighlightIcon.SetActive(toggle);
        IsSelected = toggle;
        if (toggle)
        {
            InventoryItemController.UpdateControlsPanel(this);
        }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        ToggleHighlight(true);
    }


    public virtual void OnDeselect(BaseEventData eventData)
    {
        ToggleHighlight(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryItemController.PlayerContext.PlayerInput.currentControlScheme != InventoryItemController.PlayerContext.PlayerController.KEYBOARD_SCHEME)
            return;

        ToggleHighlight(true);
        selectable.Select();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerId != PlayerContext.PlayerIndex) return;
        ToggleHighlight(false);
    }

    public void OnDropItem(CallbackContext context)
    {
        if (!IsSelected) return;
        OnRightClick();
    }

    public virtual void OnEquipOffhand(CallbackContext context)
    {
        if (!IsSelected) return;

    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public virtual void CheckIfButtonCanBeActivated()
    {

    }
}
