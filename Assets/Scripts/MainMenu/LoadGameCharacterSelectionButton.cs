using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadGameCharacterSelectionButton : UIButton
{
    public GameObject PlayerOneIcon;
    public GameObject PlayerTwoIcon;

    public bool _isInteractable = true;

    public void EnablePlayerOneButtonIcon()
    {
        PlayerOneIcon.SetActive(true);
    }

    public void DisablePlayerOneButtonIcon()
    {
        PlayerOneIcon.SetActive(false);
    }

    public void EnablePlayerTwoButtonIcon()
    {

    }

    public override void ToggleHighlight(bool toggle)
    {
        if (!_isInteractable) return;

        HighlightIcon.SetActive(toggle);
        IsSelected = toggle;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (!_isInteractable) return;

        ToggleHighlight(true);
    }


    public override void OnDeselect(BaseEventData eventData)
    {
        if (!_isInteractable) return;

        ToggleHighlight(false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInteractable) return;

        ToggleHighlight(true);
        button.Select();
    }
}
