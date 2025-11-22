using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public class SelectableButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    public GameObject selectedShader;
    public bool isSelected;
    public Button selectable;
    public void OnSelect(BaseEventData eventData)
    {

    }

    public void OnDeselect(BaseEventData eventData)
    {
    }

    public void DeselectButton()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }
}
