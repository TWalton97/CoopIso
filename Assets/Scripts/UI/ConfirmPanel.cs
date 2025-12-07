using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public PlayerUserInterfaceController userInterfaceController;
    public GameObject ConfirmPanelGameObject;
    public Button DeclineButton;
    private Action storedAction;
    private GameObject storedSelectionObject;

    private float InputDelayTime;

    public void ToggleConfirmPanelForAction(Action action, GameObject selectionObject)
    {
        InputDelayTime = Time.time + 0.1f;
        storedSelectionObject = selectionObject;
        storedAction = action;
        ConfirmPanelGameObject.SetActive(true);
        userInterfaceController.eventSystem.SetSelectedGameObject(DeclineButton.gameObject);
    }

    public void Decline()
    {
        if (Time.time < InputDelayTime) return;

        if (storedSelectionObject != null)
            userInterfaceController.eventSystem.SetSelectedGameObject(storedSelectionObject.gameObject);
        storedSelectionObject = null;
        storedAction = null;
        ConfirmPanelGameObject.SetActive(false);
    }

    public void Accept()
    {
        if (Time.time < InputDelayTime) return;

        if (storedAction != null)
        {
            storedAction?.Invoke();
        }

        storedSelectionObject = null;
        storedAction = null;
        ConfirmPanelGameObject.SetActive(false);
    }
}
