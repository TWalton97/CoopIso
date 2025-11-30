using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    InteractionType interactionType { get; set; }
    bool isInteractable { get; set; }
    string interactableName { get; set; }
    public void OnInteract(PlayerContext playerContext, int playerIndex);
    public string GetInteractableName();
}
