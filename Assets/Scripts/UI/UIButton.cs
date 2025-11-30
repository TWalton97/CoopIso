using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    public bool IsSelected;
    public GameObject HighlightIcon;

    public Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void ToggleHighlight(bool toggle)
    {
        HighlightIcon.SetActive(toggle);
        IsSelected = toggle;
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
        ToggleHighlight(true);
        button.Select();
    }
}
