using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsSelected;
    public Image TabImage;
    public Color ActivatedColor, InactiveColor;

    public Button button;

    private PlayerUserInterfaceController playerUserInterfaceController;

    void Awake()
    {
        button = GetComponent<Button>();
        playerUserInterfaceController = GetComponentInParent<PlayerUserInterfaceController>();
    }

    public virtual void ToggleHighlight(bool toggle)
    {
        IsSelected = toggle;
        if (toggle)
        {
            TabImage.color = ActivatedColor;
        }
        else
        {
            TabImage.color = InactiveColor;
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
        if (playerUserInterfaceController.playerContext.PlayerInput.currentControlScheme != playerUserInterfaceController.playerContext.PlayerController.KEYBOARD_SCHEME)
            return;

        if (!button.interactable)
            return;

        ToggleHighlight(true);
        button.Select();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (playerUserInterfaceController.playerContext.PlayerInput.currentControlScheme != playerUserInterfaceController.playerContext.PlayerController.KEYBOARD_SCHEME)
            return;

        if (!button.interactable)
            return;

        ToggleHighlight(false);
    }
}
