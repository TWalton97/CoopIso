using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class ItemButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public PlayerContext PlayerContext;
    public InventoryController InventoryController;
    public ItemData ItemData;
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

    protected string ButtonID;

    void Awake()
    {
        selectable = GetComponent<Button>();
    }

    public abstract void InitializeItemButton(InventoryItemController inventoryItemController, ItemData itemData, string buttonID, PlayerContext playerContext, bool isEquipped = false);
    public abstract void OnRightClick();
    public abstract void OnLeftClick();
    public abstract void ActivateButton();

    public virtual void OnSelect(BaseEventData eventData)
    {
        HighlightIcon.SetActive(true);
    }


    public virtual void OnDeselect(BaseEventData eventData)
    {
        HighlightIcon.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        HighlightIcon.SetActive(true);
        selectable.Select();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        HighlightIcon.SetActive(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }
}
