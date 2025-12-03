using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public InteractionUI Player1UI;
    public InteractionUI Player2UI;

    [System.Serializable]
    public class InteractionUI
    {
        public GameObject InteractionGameObject;
        public TMP_Text InteractionText;
    }

    public void EnableInteractionUI(int playerIndex, InteractionType interactionType, string interactableName, string interactionKey)
    {
        switch (playerIndex)
        {
            case 0:
                Player1UI.InteractionText.text = "P" + (playerIndex + 1).ToString() + " Press " + interactionKey + " to " + ReturnInteractionString(interactionType) + interactableName;
                Player1UI.InteractionGameObject.SetActive(true);
                break;
            case 1:
                Player2UI.InteractionText.text = "P" + (playerIndex + 1).ToString() + " Press " + interactionKey + " to " + ReturnInteractionString(interactionType) + interactableName;
                Player2UI.InteractionGameObject.SetActive(true);
                break;
        }
    }

    public void DisableInteractionUI(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                Player1UI.InteractionGameObject.SetActive(false);
                break;
            case 1:
                Player2UI.InteractionGameObject.SetActive(false);
                break;
        }
    }

    private string ReturnInteractionString(InteractionType interactionType)
    {
        switch (interactionType)
        {
            case InteractionType.PickUp:
                return "pick up ";
            case InteractionType.Open:
                return "open ";
            case InteractionType.Close:
                return "close ";
            case InteractionType.Travel:
                return "travel to ";
        }
        return "";
    }
}

public enum InteractionType
{
    PickUp,
    Open,
    Close,
    Travel,
    TalkTo,
}
