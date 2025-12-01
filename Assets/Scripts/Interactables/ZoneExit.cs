using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneExit : MonoBehaviour, IInteractable
{
    public string zoneName;
    public string interactableName { get => zoneName; set => zoneName = value; }

    public InteractionType InteractionType;
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    private bool _isInteractable = true;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }

    public SceneGroup SceneGroupToLoad;
    public int SpawnPointID;

    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        PlayerJoinManager.Instance.TargetSpawnID = SpawnPointID;
        SceneLoadingManager.Instance.LoadSceneGroup(SceneGroupToLoad);
    }

    public string GetInteractableName()
    {
        return interactableName;
    }
}
