using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour, IInteractable
{
    public List<NewPlayerController> NearbyPlayers;
    public float RespawnRadius;

    private string itemName = "Check Point";
    public string interactableName { get => itemName; set => itemName = value; }

    public InteractionType InteractionType;
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    private bool _isInteractable = true;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NewPlayerController controller))
        {
            if (!NearbyPlayers.Contains(controller) && !controller.IsDead)
            {
                NearbyPlayers.Add(controller);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NewPlayerController controller))
        {
            if (NearbyPlayers.Contains(controller))
            {
                NearbyPlayers.Remove(controller);
            }
        }
    }

    void Update()
    {
        if (NearbyPlayers.Count == 0) return;

        foreach (NewPlayerController controller in NearbyPlayers)
        {
            if (controller.IsDead)
            {
                NearbyPlayers.Remove(controller);
                return;
            }
        }

        foreach (NewPlayerController controller in PlayerJoinManager.Instance.playerControllers.Values)
        {
            if (controller.IsDead)
            {
                RespawnPlayer(controller);
            }
        }
    }

    public void RespawnPlayer(NewPlayerController controller)
    {
        controller.transform.position = ReturnPositionInRadius();
        controller.HealthController.IsDead = false;
        controller.HealthController.Heal(controller.HealthController.MaximumHealth / 2);
        controller.IsDead = false;
        controller.Animator.SetBool("Dead", false);
    }

    private Vector3 ReturnPositionInRadius()
    {
        Vector3 insideUnitCircle = Random.insideUnitCircle * RespawnRadius;
        insideUnitCircle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        return transform.position + insideUnitCircle;
    }

    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        ZoneManager zoneManager = ZoneManager.Instance;
        if (zoneManager != null)
        {
            zoneManager.GenerateZoneData(SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene().name);
        }
    }

    public string GetInteractableName()
    {
        return interactableName;
    }
}
