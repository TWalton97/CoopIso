using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float interactionRange;
    private PlayerInputController playerInputController;
    private int playerIndex;

    private Collider[] overlappingColliders;                        //Buffer for storing all nearby colliders
    private List<Collider> nearbyInteractableColliders = new();     //After we sort the buffer for valid IIntertactables
    private IInteractable nearestInteractable;                      //Sort valid IIntertactables for the nearest one

    private void Awake()
    {
        playerInputController = GetComponentInParent<PlayerInputController>();
        playerIndex = playerInputController.playerIndex;
        overlappingColliders = new Collider[10];
    }

    void Start()
    {
        StartCoroutine(InteractablesLoop());
    }
    private void DisplayInteractUI()
    {
        InteractionManager.Instance.EnableInteractionUI(playerIndex, nearestInteractable.interactionType, nearestInteractable.interactableName, GetInteractionKeyString());
    }

    private void HideInteractUI()
    {
        InteractionManager.Instance.DisableInteractionUI(playerIndex);
    }

    public void Interact()
    {
        if (nearestInteractable != null)
        {
            nearestInteractable.OnInteract(playerIndex);
        }
    }

    private string GetInteractionKeyString()
    {
        if (playerInputController.playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            return playerInputController.playerInputActions.Player.Interact.GetBindingDisplayString(group: "Keyboard&Mouse");
        }
        else if (playerInputController.playerInput.currentControlScheme == "Gamepad")
        {
            return playerInputController.playerInputActions.Player.Interact.GetBindingDisplayString(group: "Gamepad");
        }
        return "No Input Found";
    }

    private IEnumerator InteractablesLoop()
    {
        while (true)
        {
            CheckForInteractables();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckForInteractables()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactionRange, overlappingColliders, Physics.AllLayers);
        if (numColliders == 0) return;

        nearbyInteractableColliders.Clear();

        for (int i = 0; i < numColliders; i++)
        {
            if (overlappingColliders[i].TryGetComponent(out IInteractable interactable) && interactable.isInteractable)
            {
                nearbyInteractableColliders.Add(overlappingColliders[i]);
            }
        }

        if (nearbyInteractableColliders.Count == 0)
        {
            nearestInteractable = null;
            HideInteractUI();
        }
        else
        {
            if (nearbyInteractableColliders.Count > 1)
            {
                nearestInteractable = ReturnNearestInteractable();
            }
            else
            {
                nearestInteractable = nearbyInteractableColliders[0].GetComponent<IInteractable>();
            }

            if (nearestInteractable != null)
                DisplayInteractUI();
        }
    }

    private IInteractable ReturnNearestInteractable()
    {
        float distFromInteractable = (interactionRange * 2) + 0.1f;
        Collider nearestCollider = null;
        foreach (Collider coll in nearbyInteractableColliders)
        {
            float dist = UtilityMathFunctions.SquaredDistance(transform.position, coll.transform.position);
            if (dist < distFromInteractable)
            {
                nearestCollider = coll;
                distFromInteractable = dist;
            }
        }
        if (nearestCollider == null) return null;
        return nearestCollider.GetComponent<IInteractable>();
    }


}
